export const mapNamesFromArray = <T extends { name: string }>(
	array: T[] | undefined
) => (array ? array.map((item) => item.name) : []);
