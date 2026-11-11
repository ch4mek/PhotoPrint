import { getFullName } from '@/helpers/utils';
import { LookupUserResponse } from '@/types/userInfo';

export const normalizeUser =
	(
		isFetching: boolean,
		users?: Array<Pick<LookupUserResponse, 'id' | 'firstName' | 'lastName'>>
	) =>
	() =>
		isFetching || !users?.length
			? []
			: users.map(({ id, lastName, firstName }) => ({
					id,
					name: getFullName(firstName, lastName),
					description: getFullName(firstName, lastName),
			  }));
