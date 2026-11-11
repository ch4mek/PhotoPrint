export const getItemFromStorage = (
	key: string,
	userId?: number,
	storage = sessionStorage
) => JSON.parse(storage.getItem(userId ? `${key}/${userId}` : key) as string);

export const setItemToStorage = <T>(
	key: string,
	data: T,
	storage = sessionStorage
) => {
	storage.setItem(key, JSON.stringify(data));
};
