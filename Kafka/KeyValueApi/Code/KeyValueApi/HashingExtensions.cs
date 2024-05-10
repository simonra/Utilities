public static class HashingExtensions
{
    public static string GetHashString(this byte[] input)
    {
        /* Consider maybe using another hashing algorithm in the future, for the encrypted use case if for some reason that should prove necessary.
        For now all comparisons go directly towards the value, and the hash is mostly to provide a loose bucket. So internally, there is no need to
        have a super low probability of collisions. It's not like this is for checking passwords, where accepting a wide variety of passwords (hash
        matches) would be an issue. The main issue here is using the hash to try to reverse engineer what values might be stored in the case where
        the operator has bothered to enable encryption of the storage. By using a hashing algorithm with higher probabilities of collisions (though
        not so high as to dilute the buckets so far that performance is affected), and further chopping of most of the hash data and keeping only the
        first couple of chars/bytes, an attacker should have lost most of the chances of reliably guessing what might have actually been stored. */
        var hashBytes = System.IO.Hashing.Crc32.Hash(input);
        var hashHex = Convert.ToHexString(hashBytes).ToLowerInvariant();
        return hashHex.Substring(0,6);
    }
}
