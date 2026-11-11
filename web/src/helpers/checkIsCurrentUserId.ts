export const checkIsCurrentUserId = (
	currentUserId: number,
	userId: string | number | undefined
): boolean => {
	if (!userId) {
		return false;
	}

	return currentUserId === +userId;
};
