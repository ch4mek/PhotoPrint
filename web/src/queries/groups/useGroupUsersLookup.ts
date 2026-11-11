import axios from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse } from '../types';

import { GroupQueriesKeys } from '@/constants/queriesKeys';
import { departmentGroupUsersEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { GroupUsersContract } from './types';

const TOP_LIMIT = 10000;

export const useGroupUsersLookup = ({ groupId }: { groupId: number }) =>
	useQueryWithCallback<Array<GroupUsersContract>>({
		queryKey: [GroupQueriesKeys.groupMembersLookup, groupId],
		queryFn: async () => {
			const query = buildQuery({
				top: TOP_LIMIT,
				skip: 0,
			});

			const { data } = await axios.get<ByPageResponse<GroupUsersContract>>(
				`${departmentGroupUsersEndpoint(groupId)}${query}`
			);

			return data.items;
		},
	});
