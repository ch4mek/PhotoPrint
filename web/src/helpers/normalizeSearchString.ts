export const normalizeSearchString = (string?: string): string | undefined =>
	string ? string.replace(/['&]/g, (match) => `\\${match}`) : undefined;
