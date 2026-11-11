const LAST_COMMA_REGEX = /, ([^,]*)$/;
const LAST_COMMA_REPLACEMENT = ' and $1';

export const formatAllowedFileTypesToAccept = (extensions: string[]) =>
	extensions.map((extension) => `.${extension}`).join(', ');

export const formatAllowedFileTypesToShow = (
	extensions: string[],
	includeAndWordSeparator?: boolean
) => {
	const mappedTypes = extensions
		.map((extension) => extension.toUpperCase())
		.join(', ');

	return includeAndWordSeparator
		? mappedTypes.replace(LAST_COMMA_REGEX, LAST_COMMA_REPLACEMENT)
		: mappedTypes;
};
