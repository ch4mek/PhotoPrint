export const getDomenFromUrl = (url: string) => {
	try {
		const parsedUrl = new URL(url);

		return parsedUrl.hostname;
	} catch (error) {
		return '';
	}
};
