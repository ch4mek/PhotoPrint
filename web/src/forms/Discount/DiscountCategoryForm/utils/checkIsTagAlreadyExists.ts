export const checkIsTagAlreadyExists = (
	tagsData: string[],
	newTagName: string
): boolean =>
	tagsData.some((tag) => tag.toLowerCase() === newTagName.toLowerCase());
