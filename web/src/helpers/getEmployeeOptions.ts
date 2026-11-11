import { LookupUserResponse } from '@/types/userInfo';

import { getFullName } from './formatName';

export const getEmployeeOptions = (
	data: LookupUserResponse[] | undefined,
	isFetching: boolean
) =>
	isFetching || !data?.length
		? []
		: data.map(({ id, lastName, firstName, positionDescription }) => ({
				id,
				name: getFullName(firstName, lastName),
				positionDescription,
		  }));
