using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySupport
{
    /// <summary>
    /// Practical EKE (Encrypted Key Exchange), uses variation of DH-EKE (or this comment is outdated).
    /// 
    /// NOTE! Actually first implementation isn't DH-EKE, but just EKE with shared secret hashed to Key & IV for AES.
    /// 
    /// Anyway ground-up implemented EKE with following steps more or less:
    /// 1. Alice generates public/private key pair unique for the session Pa (private) and P'a (public) Alice encrypts public key using S and sends it to Bob.
    /// 2. Bob (knowing S) decrypts Alices message and recovers Alice's public key P'a. Bob generates random session key K. Bob encrypts K with Alice's public key P'a and sends it to Alice.
    /// 3. Alice decrypts the message and obtains K. Alice generates random string Ra, encrypts it with K and sends to bob
    /// 4. Bob decrypts the message to obtain Ra, generates another random Rb, encrypts both with K and sends the encrypted message to Alice.
    /// 5. Alice decrypts message, verifies her own Ra being valid in the message. She encrypts only Rb with K and sends to Bob.
    /// 6. Bob decrypts Rb and verifies his own Rb being valid.
    /// </summary>
    public class TheBallEKE
    {
        public static void TestExecution()
        {
            TheBallEKE bobInstance = new TheBallEKE();
            bobInstance.InitiateCurrentSymmetricFromSecret("testsecret2");
            EKEBob bob = new EKEBob(ekeContext: bobInstance);

            TheBallEKE aliceInstance = new TheBallEKE();
            aliceInstance.InitiateCurrentSymmetricFromSecret("testsecret2");
            EKEAlice alice = new EKEAlice(ekeContext: aliceInstance);
            // Hook message senders
            alice.SendMessageToOtherParty = msg =>
                {
                    bob.LatestMessageFromOtherParty = msg;
                };
            bob.SendMessageToOtherParty = msg =>
                {
                    alice.LatestMessageFromOtherParty = msg;
                };
            bool ekeInProgress = true;
            while (ekeInProgress)
            {
                alice.PerformNextAction();
                bob.PerformNextAction();
                ekeInProgress = alice.IsDoneWithProtocol == false || bob.IsDoneWithProtocol == false;
            }
            bool ekeSuccess = true;
        }

        public delegate void NegotiationAction();
        public delegate Task NegotiationActionAsync();

        private SymmetricSupport SharedSecretEnc;
        private const int RSAKEYLENGTH = 2048;

        public class EKEAlice : INegotiationProtocolMember
        {
            private TheBallEKE EKEContext;

            public EKEAlice(TheBallEKE ekeContext, bool isAsync = false)
            {
                EKEContext = ekeContext;
                AlicesActions = new NegotiationAction[]
                    {
                        Alice1_1_GenerateKeyPair, Alice1_2_EncryptPublicKeyWithS, Alice1_3_SendEncryptedPublicKeyToBob,

                        Alice3_0_GetEncryptedSessionKeyFromBob, Alice3_1_DecryptSessionKey,
                        Alice3_2_GenerateAliceRandomValue,
                        Alice3_3_EncryptAliceRandomValueWithSessionKey, Alice3_4_SendEncryptedAliceRandomToBob,

                        Alice5_0_GetAlicesRandomWithBobsRandomEncryptedFromBob, Alice5_1_DecryptBothRandoms,
                        Alice5_2_VerifyAliceRandomInCombinedRandom,
                        Alice5_3_ExtractBobsRandom, Alice5_4_EncryptBobsRandom, Alice5_5_SendEncryptedBobsRandomToBob,

                        AliceX_DoneWithEKE
                    };
                AlicesActionsAsync = new NegotiationActionAsync[]
                    {
                        Alice1_1_GenerateKeyPairAsync, Alice1_2_EncryptPublicKeyWithSAsync, Alice1_3_SendEncryptedPublicKeyToBobAsync,

                        Alice3_0_GetEncryptedSessionKeyFromBobAsync, Alice3_1_DecryptSessionKeyAsync,
                        Alice3_2_GenerateAliceRandomValueAsync,
                        Alice3_3_EncryptAliceRandomValueWithSessionKeyAsync, Alice3_4_SendEncryptedAliceRandomToBobAsync,

                        Alice5_0_GetAlicesRandomWithBobsRandomEncryptedFromBobAsync, Alice5_1_DecryptBothRandomsAsync,
                        Alice5_2_VerifyAliceRandomInCombinedRandomAsync,
                        Alice5_3_ExtractBobsRandomAsync, Alice5_4_EncryptBobsRandomAsync, Alice5_5_SendEncryptedBobsRandomToBobAsync,

                        AliceX_DoneWithEKEAsync
                    };
                CurrentActionIndex = 0;
                WaitForOtherParty = false;
                IsAsync = isAsync;
            }

            private int CurrentActionIndex;

            private void AliceX_DoneWithEKE()
            {
                IsDoneWithProtocol = true;
            }

            private NegotiationAction[] AlicesActions; 
            private NegotiationActionAsync[] AlicesActionsAsync;


            public Action<byte[]> SendMessageToOtherParty { get; set; }
            public Func<byte[], Task> SendMessageToOtherPartyAsync { get; set; }

            public List<byte[]> NegotiationResults
            {
                get
                {
                    if (!IsDoneWithProtocol)
                        return null;
                    return new List<byte[]>() {SessionKeyEnc.CurrentKey};
                }
            }

            private SymmetricSupport SessionKeyEnc;

            //public byte[] SharedSecret;
            private RSACryptoServiceProvider PublicAndPrivateKeys;
            private byte[] EncryptedPublicKey;
            private byte[] EncryptedSessionKey;
            private byte[] AlicesRandom;
            private byte[] AlicesRandomEncrypted;
            private byte[] AlicesRandomWithBobsRandomEncrypted;
            private byte[] AlicesRandomWithBobsRandom;
            private byte[] BobsRandom;
            private byte[] BobsRandomEncrypted;
            public bool WaitForOtherParty { get; private set; }
            public bool IsDoneWithProtocol { get; private set; }

            private byte[] latestMessageFromOtherParty;
            private bool IsAsync;

            public byte[] LatestMessageFromOtherParty
            {
                private get { return latestMessageFromOtherParty; }
                set
                {
                    latestMessageFromOtherParty = value;
                    WaitForOtherParty = false;
                }
            }

            private void Alice5_5_SendEncryptedBobsRandomToBob()
            {
                SendMessageToOtherParty(BobsRandomEncrypted);
            }

            private void Alice5_4_EncryptBobsRandom()
            {
                BobsRandomEncrypted = SessionKeyEnc.EncryptData(BobsRandom);
            }

            private void Alice5_3_ExtractBobsRandom()
            {
                BobsRandom = AlicesRandomWithBobsRandom.Skip(16).ToArray();
            }

            private void Alice5_2_VerifyAliceRandomInCombinedRandom()
            {
                var alicesRandomExtracted = AlicesRandomWithBobsRandom.Take(16);
                if (AlicesRandom.SequenceEqual(alicesRandomExtracted) == false)
                    throw new SecurityException("EKE negotiation failed");
            }

            private void Alice5_0_GetAlicesRandomWithBobsRandomEncryptedFromBob()
            {
                AlicesRandomWithBobsRandomEncrypted = LatestMessageFromOtherParty;
            }
            
            private void Alice5_1_DecryptBothRandoms()
            {
                AlicesRandomWithBobsRandom = SessionKeyEnc.DecryptData(AlicesRandomWithBobsRandomEncrypted);
            }

            private void Alice3_4_SendEncryptedAliceRandomToBob()
            {
                SendMessageToOtherParty(AlicesRandomEncrypted);
                WaitForOtherParty = true;
            }

            private void Alice3_3_EncryptAliceRandomValueWithSessionKey()
            {
                AlicesRandomEncrypted = SessionKeyEnc.EncryptData(AlicesRandom);
            }

            private void Alice3_2_GenerateAliceRandomValue()
            {
                AlicesRandom = SymmetricSupport.GetRandomBytes(16);
            }

            private void Alice3_0_GetEncryptedSessionKeyFromBob()
            {
                EncryptedSessionKey = LatestMessageFromOtherParty;
            }

            private void Alice3_1_DecryptSessionKey()
            {
                var sessionKeyAndIV = PublicAndPrivateKeys.Decrypt(EncryptedSessionKey, false);
                SessionKeyEnc = new SymmetricSupport();
                SessionKeyEnc.InitializeFromKeyAndIV(sessionKeyAndIV);
            }

            private void Alice1_3_SendEncryptedPublicKeyToBob()
            {
                SendMessageToOtherParty(EncryptedPublicKey);
                WaitForOtherParty = true;
            }

            private void Alice1_2_EncryptPublicKeyWithS()
            {
                string rsaPublicKey = PublicAndPrivateKeys.ToXmlString(false);
                EncryptedPublicKey = EKEContext.SharedSecretEnc.EncryptString(rsaPublicKey);
            }

            private void Alice1_1_GenerateKeyPair()
            {
                using (var rsa = new RSACryptoServiceProvider(TheBallEKE.RSAKEYLENGTH))
                {
                    try
                    {
                        PublicAndPrivateKeys = rsa;
                    }
                    finally
                    {
                        rsa.PersistKeyInCsp = false;
                    }
                }
            }


            public void PerformNextAction()
            {
                if (IsAsync)
                    throw new NotSupportedException("PerformNextAction not supported on Async mode, use PerformNextActionAsync instead");
                if (IsDoneWithProtocol || WaitForOtherParty)
                    return;
                AlicesActions[CurrentActionIndex++]();
            }


            #region Async Methods

            public async Task PerformNextActionAsync()
            {
                if (!IsAsync)
                    throw new NotSupportedException("PerformNextActionAsync not supported on non-async mode, use PerformNextAction instead");
                if (IsDoneWithProtocol || WaitForOtherParty)
                    return;
                await AlicesActionsAsync[CurrentActionIndex++]();
            }

            private async Task AliceX_DoneWithEKEAsync()
            {
                AliceX_DoneWithEKE();
            }

            private async Task Alice5_5_SendEncryptedBobsRandomToBobAsync()
            {
                await SendMessageToOtherPartyAsync(BobsRandomEncrypted);
            }

            private async Task Alice5_4_EncryptBobsRandomAsync()
            {
                Alice5_4_EncryptBobsRandom();
            }

            private async Task Alice5_3_ExtractBobsRandomAsync()
            {
                Alice5_3_ExtractBobsRandom();
            }

            private async Task Alice5_2_VerifyAliceRandomInCombinedRandomAsync()
            {
                Alice5_2_VerifyAliceRandomInCombinedRandom();
            }

            private async Task Alice5_0_GetAlicesRandomWithBobsRandomEncryptedFromBobAsync()
            {
                Alice5_0_GetAlicesRandomWithBobsRandomEncryptedFromBob();
            }

            private async Task Alice5_1_DecryptBothRandomsAsync()
            {
                Alice5_1_DecryptBothRandoms();
            }

            private async Task Alice3_4_SendEncryptedAliceRandomToBobAsync()
            {
                await SendMessageToOtherPartyAsync(AlicesRandomEncrypted);
                WaitForOtherParty = true;
            }

            private async Task Alice3_3_EncryptAliceRandomValueWithSessionKeyAsync()
            {
                Alice3_3_EncryptAliceRandomValueWithSessionKey();
            }

            private async Task Alice3_2_GenerateAliceRandomValueAsync()
            {
                Alice3_2_GenerateAliceRandomValue();
            }

            private async Task Alice3_0_GetEncryptedSessionKeyFromBobAsync()
            {
                Alice3_0_GetEncryptedSessionKeyFromBob();
            }

            private async Task Alice3_1_DecryptSessionKeyAsync()
            {
                Alice3_1_DecryptSessionKey();
            }

            private async Task Alice1_3_SendEncryptedPublicKeyToBobAsync()
            {
                await SendMessageToOtherPartyAsync(EncryptedPublicKey);
                WaitForOtherParty = true;
            }

            private async Task Alice1_2_EncryptPublicKeyWithSAsync()
            {
                Alice1_2_EncryptPublicKeyWithS();
            }

            private async Task Alice1_1_GenerateKeyPairAsync()
            {
                Alice1_1_GenerateKeyPair();
            }

            #endregion
        }

        public class EKEBob : INegotiationProtocolMember
        {
            private TheBallEKE EKEContext;
            private NegotiationAction[] BobsActions;
            private NegotiationActionAsync[] BobsActionsAsync;


            public EKEBob(TheBallEKE ekeContext, bool isAsync = false)
            {
                EKEAlice al;
                EKEContext = ekeContext;
                BobsActions = new NegotiationAction[]
                    {
                        Bob2_0_GetAlicesEncryptedPublicKeyFromAlice, Bob2_1_DecryptAlicesEncryptedPublicKey,
                        Bob2_2_GenerateRandomSessionKeyWithIV, Bob2_3_EncryptSessionKey,
                        Bob2_4_SendEncryptedSessionKeyToAlice,

                        Bob4_0_GetAlicesRandomEncryptedFromAlice, Bob4_1_DecryptAlicesEncryptedRandom,
                        Bob4_2_GenerateBobsRandomAndCombineWithAlicesRandom, Bob4_3_SendBothRandomsEncryptedToAlice,

                        Bob6_0_GetBobsRandomEncryptedFromAlice, Bob6_1_DecryptBobsRandomFromAlice,
                        Bob6_2_VerifyBobsRandom,

                        BobX_DoneWithEKE
                    };
                BobsActionsAsync = new NegotiationActionAsync[]
                    {
                        Bob2_0_GetAlicesEncryptedPublicKeyFromAliceAsync, Bob2_1_DecryptAlicesEncryptedPublicKeyAsync,
                        Bob2_2_GenerateRandomSessionKeyWithIVAsync, Bob2_3_EncryptSessionKeyAsync,
                        Bob2_4_SendEncryptedSessionKeyToAliceAsync,

                        Bob4_0_GetAlicesRandomEncryptedFromAliceAsync, Bob4_1_DecryptAlicesEncryptedRandomAsync,
                        Bob4_2_GenerateBobsRandomAndCombineWithAlicesRandomAsync, Bob4_3_SendBothRandomsEncryptedToAliceAsync,

                        Bob6_0_GetBobsRandomEncryptedFromAliceAsync, Bob6_1_DecryptBobsRandomFromAliceAsync,
                        Bob6_2_VerifyBobsRandomAsync,

                        BobX_DoneWithEKEAsync
                    };

                CurrentActionIndex = 0;
                WaitForOtherParty = true;
                IsAsync = isAsync;
            }

            private bool IsAsync;

            private void BobX_DoneWithEKE()
            {
                IsDoneWithProtocol = true;
            }

            public Action<byte[]> SendMessageToOtherParty { get; set;  }
            public Func<byte[], Task> SendMessageToOtherPartyAsync { get; set; }

            public List<byte[]> NegotiationResults
            {
                get
                {
                    if (!IsDoneWithProtocol)
                        return null;
                    return new List<byte[]>() { SessionKeyEnc.CurrentKey };
                }
            }

            private SymmetricSupport SessionKeyEnc;

            //public byte[] SharedSecret;
            private byte[] AlicesEncryptedPublicKey;
            private RSACryptoServiceProvider AlicesPublicKey;
            private byte[] EncryptedSessionKey;
            private byte[] AlicesEncryptedRandom;
            private byte[] AlicesRandom;
            private byte[] BobsRandom;
            private byte[] AlicesRandomWithBobsRandom;
            private byte[] AlicesRandomWithBobsRandomEncrypted;
            private byte[] BobsRandomFromAliceEncrypted;
            private byte[] BobsRandomFromAlice;

            private byte[] latestMessageFromOtherParty;
            public byte[] LatestMessageFromOtherParty
            {
                private get { return latestMessageFromOtherParty; }
                set
                {
                    latestMessageFromOtherParty = value;
                    WaitForOtherParty = false;
                }
            }

            public bool WaitForOtherParty { get; private set; }
            public bool IsDoneWithProtocol { get; private set; }
            private int CurrentActionIndex;

            private void Bob6_2_VerifyBobsRandom()
            {
                if (BobsRandom.SequenceEqual(BobsRandomFromAlice) == false)
                    throw new SecurityException("EKE negotiation failed");
            }

            private void Bob6_1_DecryptBobsRandomFromAlice()
            {
                BobsRandomFromAlice = SessionKeyEnc.DecryptData(BobsRandomFromAliceEncrypted);
            }

            private void Bob6_0_GetBobsRandomEncryptedFromAlice()
            {
                BobsRandomFromAliceEncrypted = LatestMessageFromOtherParty;
            }

            private void Bob4_3_SendBothRandomsEncryptedToAlice()
            {
                SendMessageToOtherParty(AlicesRandomWithBobsRandomEncrypted);
                WaitForOtherParty = true;
            }

            private void Bob4_2_GenerateBobsRandomAndCombineWithAlicesRandom()
            {
                BobsRandom = SymmetricSupport.GetRandomBytes(16);
                AlicesRandomWithBobsRandom = AlicesRandom.Concat(BobsRandom).ToArray();
                AlicesRandomWithBobsRandomEncrypted = SessionKeyEnc.EncryptData(AlicesRandomWithBobsRandom);
            }

            private void Bob4_1_DecryptAlicesEncryptedRandom()
            {
                AlicesRandom = SessionKeyEnc.DecryptData(AlicesEncryptedRandom);
            }

            private void Bob4_0_GetAlicesRandomEncryptedFromAlice()
            {
                AlicesEncryptedRandom = LatestMessageFromOtherParty;
            }

            private void Bob2_4_SendEncryptedSessionKeyToAlice()
            {
                SendMessageToOtherParty(EncryptedSessionKey);
                WaitForOtherParty = true;
            }

            private void Bob2_3_EncryptSessionKey()
            {
                byte[] sessionKeyWithIV = SessionKeyEnc.GetKeyWithIV();
                var result = AlicesPublicKey.Encrypt(sessionKeyWithIV, false);
                EncryptedSessionKey = result;
            }

            private void Bob2_2_GenerateRandomSessionKeyWithIV()
            {
                SessionKeyEnc = new SymmetricSupport();
                SessionKeyEnc.InitializeNew();
            }

            private void Bob2_1_DecryptAlicesEncryptedPublicKey()
            {
                string alicesPublicKey = EKEContext.SharedSecretEnc.DecryptString(AlicesEncryptedPublicKey);
                AlicesPublicKey = new RSACryptoServiceProvider();
                AlicesPublicKey.FromXmlString(alicesPublicKey);
            }

            private void Bob2_0_GetAlicesEncryptedPublicKeyFromAlice()
            {
                AlicesEncryptedPublicKey = LatestMessageFromOtherParty;
            }


            public void PerformNextAction()
            {
                if(IsAsync)
                    throw new NotSupportedException("PerformNextAction not supported on Async mode, use PerformNextActionAsync instead");
                if (IsDoneWithProtocol || WaitForOtherParty)
                    return;
                BobsActions[CurrentActionIndex++]();
            }


            #region Async Methods

            public async Task PerformNextActionAsync()
            {
                if (!IsAsync)
                    throw new NotSupportedException("PerformNextActionAsync not supported on non-async mode, use PerformNextAction instead");
                if (IsDoneWithProtocol || WaitForOtherParty)
                    return;
                await BobsActionsAsync[CurrentActionIndex++]();
            }



            private async Task BobX_DoneWithEKEAsync()
            {
                IsDoneWithProtocol = true;
            }

            private async Task Bob6_2_VerifyBobsRandomAsync()
            {
                if (BobsRandom.SequenceEqual(BobsRandomFromAlice) == false)
                    throw new SecurityException("EKE negotiation failed");
            }

            private async Task Bob6_1_DecryptBobsRandomFromAliceAsync()
            {
                BobsRandomFromAlice = SessionKeyEnc.DecryptData(BobsRandomFromAliceEncrypted);
            }

            private async Task Bob6_0_GetBobsRandomEncryptedFromAliceAsync()
            {
                BobsRandomFromAliceEncrypted = LatestMessageFromOtherParty;
            }

            private async Task Bob4_3_SendBothRandomsEncryptedToAliceAsync()
            {
                await SendMessageToOtherPartyAsync(AlicesRandomWithBobsRandomEncrypted);
                WaitForOtherParty = true;
            }

            private async Task Bob4_2_GenerateBobsRandomAndCombineWithAlicesRandomAsync()
            {
                BobsRandom = SymmetricSupport.GetRandomBytes(16);
                AlicesRandomWithBobsRandom = AlicesRandom.Concat(BobsRandom).ToArray();
                AlicesRandomWithBobsRandomEncrypted = SessionKeyEnc.EncryptData(AlicesRandomWithBobsRandom);
            }

            private async Task Bob4_1_DecryptAlicesEncryptedRandomAsync()
            {
                AlicesRandom = SessionKeyEnc.DecryptData(AlicesEncryptedRandom);
            }

            private async Task Bob4_0_GetAlicesRandomEncryptedFromAliceAsync()
            {
                AlicesEncryptedRandom = LatestMessageFromOtherParty;
            }

            private async Task Bob2_4_SendEncryptedSessionKeyToAliceAsync()
            {
                await SendMessageToOtherPartyAsync(EncryptedSessionKey);
                WaitForOtherParty = true;
            }

            private async Task Bob2_3_EncryptSessionKeyAsync()
            {
                byte[] sessionKeyWithIV = SessionKeyEnc.GetKeyWithIV();
                var result = AlicesPublicKey.Encrypt(sessionKeyWithIV, false);
                EncryptedSessionKey = result;
            }

            private async Task Bob2_2_GenerateRandomSessionKeyWithIVAsync()
            {
                SessionKeyEnc = new SymmetricSupport();
                SessionKeyEnc.InitializeNew();
            }

            private async Task Bob2_1_DecryptAlicesEncryptedPublicKeyAsync()
            {
                string alicesPublicKey = EKEContext.SharedSecretEnc.DecryptString(AlicesEncryptedPublicKey);
                AlicesPublicKey = new RSACryptoServiceProvider();
                AlicesPublicKey.FromXmlString(alicesPublicKey);
            }

            private async Task Bob2_0_GetAlicesEncryptedPublicKeyFromAliceAsync()
            {
                AlicesEncryptedPublicKey = LatestMessageFromOtherParty;
            }
            #endregion
        }


        public void InitiateCurrentSymmetricFromSecret(string textvalue)
        {
            SharedSecretEnc = new SymmetricSupport();
            SharedSecretEnc.InitializeFromSharedSecret(textvalue);
        }


    }
}
