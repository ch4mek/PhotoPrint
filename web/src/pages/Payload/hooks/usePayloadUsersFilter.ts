import { useMemo } from 'react';

import { useToast } from '@/context/Toast';
import { normalizeUser } from '@/helpers/normalizeUser';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useTeamboardUsersLookup } from '@/queries/teamboard/useTeamboardUsersLookup';

export const usePayloadUsersFilter = () => {
	const toast = useToast();

	const { data, isFetching } = useTeamboardUsersLookup(
		{
			onError: createOnErrorHandler(toast),
		},
		true
	);

	return useMemo(normalizeUser(data, isFetching), [isFetching]);
};
