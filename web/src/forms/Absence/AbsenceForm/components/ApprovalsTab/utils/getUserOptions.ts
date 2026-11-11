import { getFullName } from '@/helpers/formatName';
import { AcceptPolicesItem, AcceptPolicesUser } from '@/types/absences';
import { LookupUserResponse } from '@/types/userInfo';

export const getUserOptions = (
	users: LookupUserResponse[] | undefined,
	fieldsValues: AcceptPolicesItem[],
	isSubstitute: boolean | undefined = false,
	currentId?: string | number | undefined
): AcceptPolicesUser[] => {
	if (!users) {
		return [];
	}

	if (isSubstitute) {
		return users.reduce<AcceptPolicesUser[]>((acc, user) => {
			if (currentId && user.id === currentId) {
				return acc;
			}

			const userName = getFullName(user.firstName, user.lastName);
			acc.push({ name: userName, id: user.id });

			return acc;
		}, []);
	}
	const usedIdsSet = new Set(
		fieldsValues
			.flatMap((item) => [item.user?.id])
			.filter(Boolean)
			.concat(currentId ? [currentId] : [])
	);

	return users.reduce<AcceptPolicesUser[]>((acc, user) => {
		const userName = getFullName(user.firstName, user.lastName);

		if (!usedIdsSet.has(user.id)) {
			acc.push({ name: userName, id: user.id });
		}

		return acc;
	}, []);
};
