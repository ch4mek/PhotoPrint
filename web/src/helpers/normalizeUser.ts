import { getFullName } from '@/helpers/utils';
import { LookupUserResponse } from '@/types/userInfo';

export const normalizeUser =
	(users: LookupUserResponse[] | undefined, isFetching: boolean) => () =>
		isFetching || !users?.length
			? []
			: users.map(({ id, lastName, firstName, firstNameRu, lastNameRu }) => ({
					id,
					name: getFullName(firstName, lastName, firstNameRu, lastNameRu),
			  }));
