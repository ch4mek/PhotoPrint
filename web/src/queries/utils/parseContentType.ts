export const parseContentType = (contentType?: string): string | undefined =>
	contentType?.split(';')[0]?.trim();
