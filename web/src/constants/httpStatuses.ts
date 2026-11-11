export const HttpStatuses = {
	BadRequest: 400,
	Unauthorized: 401,
	Forbidden: 403,
	NotFound: 404,
	TooManyRequests: 429,
	ConflictRequest: 409,
	NoContent: 204,
};

export const NonRetryableStatuses = [
	HttpStatuses.BadRequest,
	HttpStatuses.Unauthorized,
	HttpStatuses.Forbidden,
	HttpStatuses.NotFound,
];
