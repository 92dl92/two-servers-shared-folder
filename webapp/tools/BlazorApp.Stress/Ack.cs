using BlazorApp.Shared.Dtos;

namespace BlazorApp.Stress;

/// <summary>
/// A change the API reported as saved, with what identifies it in the record's history
/// afterwards: the entry at <see cref="Version"/> must be this action by this user (each
/// client acts as its own user, so no other client can write the same entry). A comment must
/// carry this <see cref="Marker"/> as its text; an update writes it into the form data's notes.
/// </summary>
/// <param name="RecordId">The record changed.</param>
/// <param name="Version">The version the response returned.</param>
/// <param name="Action">What was done.</param>
/// <param name="UserId">Who did it (the client's "Acting as" user).</param>
/// <param name="Marker">Unique text: the comment for <see cref="RecordAction.Comment"/>, the notes for <see cref="RecordAction.Update"/>; otherwise null.</param>
/// <param name="Server">The server that answered, e.g. <c>5081</c>.</param>
/// <param name="CorrelationId">The request's <c>X-Correlation-Id</c>, to find it in the server logs.</param>
public sealed record Ack(
    Guid RecordId,
    int Version,
    RecordAction Action,
    string UserId,
    string? Marker,
    string Server,
    string CorrelationId);
