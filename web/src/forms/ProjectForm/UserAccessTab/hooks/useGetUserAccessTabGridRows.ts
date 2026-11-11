import { useMemo } from 'react';

import { UserAccessRow } from '../types';
import { mapUsersByProjectIdResponseToGridRows } from '../utils';

import { UseGetUsersByProjectIdResponse } from '@/queries/projects/types';

export const useGetUserAccessTabGridRows = (
	data: UseGetUsersByProjectIdResponse | undefined,
	handleEditClick: (projectId?: number | string) => () => void
) =>
	useMemo<Array<UserAccessRow>>(
		() =>
			data
				? mapUsersByProjectIdResponseToGridRows(data.items, handleEditClick)
				: [],
		[data?.items]
	);
