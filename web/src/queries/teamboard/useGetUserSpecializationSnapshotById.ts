import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_SPECIALIZATIONS_SNAPSHOTS } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { SpecializationSnapshot } from '@/types/teamboard';

export const useGetUserSpecializationSnapshotById = (
	snapshotId: number | undefined
) =>
	useQueryWithCallback<SpecializationSnapshot, AxiosError<ServerResponseError>>(
		{
			queryKey: [
				TeamboardQueriesKeys.userSpecializationSnapshotByDate,
				snapshotId,
			],
			queryFn: () =>
				axios
					.get<SpecializationSnapshot>(
						`${GET_USER_SPECIALIZATIONS_SNAPSHOTS}/${snapshotId}`
					)
					.then((res) => res.data),
			refetchOnWindowFocus: false,
			enabled: !!snapshotId,
		}
	);
