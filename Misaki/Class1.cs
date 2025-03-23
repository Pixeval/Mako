namespace Misaki;

public interface IMisakaBase;

public interface IIdentityInfo : IMisakaBase
{
    long Id { get; }

    string Platform { get; }

    const string Pixiv = "pixiv";
}

public interface IArtworkInfo : IIdentityInfo
{
    string Title { get; }

    string Description { get; }

    DateTimeOffset CreateDate { get; }

    DateTimeOffset UpdateDate { get; }

    DateTimeOffset ModifyDate { get; }

    IPreloadableEnumerable<IUser> Authors { get; }

    IPreloadableEnumerable<IUser> Uploaders { get; }

    SafeRating SafeRating { get; }

    ILookup<ITagCategory, ITag> Tags { get; }

    IReadOnlyList<IImageFrame> Thumbnails { get; }

    int TotalFavorite { get; }

    int TotalView { get; }

    IReadOnlyDictionary<string, object> AdditionalInfo { get; }
}

public interface IImageFrame : IMisakaBase
{
    int Width { get; }

    int Height { get; }

    ulong ByteSize { get; }

    Uri Uri { get; }
}

public interface ISingleImage : IImageFrame, IArtworkInfo
{
    /// <summary>
    /// 1 for static image, more than 1 for animated image
    /// </summary>
    int FrameCount { get; }
}

public interface IImageSet : IArtworkInfo, IPreloadableEnumerable<IArtworkInfo>
{
    int PageCount { get; }
}

public interface IImagePool : IAsyncEnumerable<IArtworkInfo>
{
}

public interface IUser : IIdentityInfo
{
    string Name { get; }

    string Description { get; }

    Uri Uri { get; }

    IReadOnlyList<IImageFrame> Avatar { get; }

    IReadOnlyDictionary<string, Uri> ContactInformation { get; }

    IReadOnlyDictionary<string, object> AdditionalInfo { get; }
}

public interface ITagCategory : IMisakaBase
{
    static ITagCategory Empty { get; } = new EmptyCategory();

    string Name { get; }

    string Description { get; }

    private readonly record struct EmptyCategory : ITagCategory
    {
        public string Name => "";

        public string Description => "";
    }
}


public interface ITag : IMisakaBase
{
    ITagCategory Category { get; }

    string Name { get; }

    string Description { get; }
}

public interface IImageUriDownloadProvider : IMisakaBase
{
    string Schema { get; }

    Task<Stream> DownloadImageAsync(ISingleImage image, Stream? destination = null);

    Task<IReadOnlyList<Stream>> DownloadAnimatedImagePreferredSeparatedAsync(ISingleImage image);
}

/// <remarks>
/// <seealso href="https://danbooru.donmai.us/wiki_pages/howto:rate"/>
/// </remarks>
public readonly record struct SafeRating(sbyte SafeRatingValue)
{
    private const sbyte NotSpecifiedValue = -1; 

    private const sbyte GeneralValue = 10; 

    private const sbyte SensitiveValue = 20; 

    private const sbyte QuestionableValue = 30; 

    private const sbyte ExplicitValue = 40; 

    private const sbyte GuroValue = 50; 

    public static SafeRating NotSpecified { get; } = new(NotSpecifiedValue);

    public static SafeRating General { get; } = new(GeneralValue);

    public static SafeRating Sensitive { get; } = new(SensitiveValue);

    public static SafeRating Questionable { get; } = new(QuestionableValue);

    /// <summary>
    /// Can be guro in booru
    /// </summary>
    public static SafeRating Explicit { get; } = new(ExplicitValue);

    public static SafeRating Guro { get; } = new(GuroValue);

    public bool IsNotSpecified => SafeRatingValue is NotSpecifiedValue;

    public bool IsGeneral => SafeRatingValue is > NotSpecifiedValue and <= GeneralValue;

    public bool IsSensitive => SafeRatingValue is > GeneralValue and <= SensitiveValue;

    public bool IsQuestionable => SafeRatingValue is > SensitiveValue and <= QuestionableValue;

    public bool IsExplicit => SafeRatingValue is > QuestionableValue and <= ExplicitValue;

    public bool IsGuro => SafeRatingValue > ExplicitValue;

    public bool IsSafe => IsGeneral || IsSensitive;

    public bool IsR17 => IsQuestionable;

    public bool IsR18 => IsExplicit;

    public bool IsR18G => IsGuro;
}
