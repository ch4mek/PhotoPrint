export const getIdFromPathname = (pathname: string) => {
	const parts = pathname.split('/');

	return parts[parts.length - 1];
};
