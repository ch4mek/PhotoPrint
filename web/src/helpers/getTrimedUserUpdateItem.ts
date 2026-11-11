import { UseUpdateUserResponse } from '@/types/userInfo';

export const getTrimedUserUpdateItem = (
	updatedUserItem: UseUpdateUserResponse
) => ({
	...updatedUserItem,
	firstName: updatedUserItem.firstName.trim(),
	lastName: updatedUserItem.lastName.trim(),
});
