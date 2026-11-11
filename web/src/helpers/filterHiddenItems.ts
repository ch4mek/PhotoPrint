// TODO: remove within MMS2-6978
export const filterHiddenItems = <T>(
	items: T[],
	hiddenItemValues: T[keyof T][],
	key: keyof T
): T[] => items.filter((item) => !hiddenItemValues.includes(item[key]));

export const filterHiddenItemsByName = <T extends { name: string }>(
	items: T[],
	hiddenItemValues: T[keyof T][]
) => filterHiddenItems(items, hiddenItemValues, 'name');
