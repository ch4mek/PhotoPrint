import { useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';

import { SEARCH_INPUT_DELAY } from '../constants';
import { UseUsersMutationsProps } from '../types';

import { UsersQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/index';
import { getTrimedUserUpdateItem } from '@/helpers/getTrimedUserUpdateItem';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { updateGridItems } from '@/helpers/updateGridItems';
import { useDebounce } from '@/hooks/useDebounce';
import { usePageWithSearch } from '@/hooks/usePageWithSearch';
import { useCreateUser } from '@/queries/useCreateUser';
import { useUpdateUser } from '@/queries/useUpdateUser';
import { useUpdateUserStatus } from '@/queries/useUpdateUserStatus';
import { UseUpdateUserResponse } from '@/types/userInfo';

export const useUsersMutations = ({
	refetch,
	refetchUsersLookup,
	page,
	rowsPerPage,
	order,
	orderBy,
}: UseUsersMutationsProps) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { search } = usePageWithSearch();
	const debouncedSearch = useDebounce(search, SEARCH_INPUT_DELAY);

	const [isEmailDuplicate, setIsEmailDuplicate] = useState(false);

	const updateUserStatusMutation = useUpdateUserStatus({
		onSuccess() {
			refetch();
		},
	});

	const { mutateAsync: onUserCreate, isPending: isUserCreating } =
		useCreateUser({
			onSuccess: () => {
				refetch();
				refetchUsersLookup();
			},
			onError: createOnErrorHandler(toast),
		});

	const { mutateAsync: onUserUpdate } = useUpdateUser(setIsEmailDuplicate, {
		onSuccess: (updatedUserItem) => {
			const validatedUserItem: UseUpdateUserResponse =
				getTrimedUserUpdateItem(updatedUserItem);

			queryClient.setQueryData(
				[
					UsersQueriesKeys.users,
					page + 1,
					rowsPerPage,
					order,
					orderBy,
					debouncedSearch,
				],
				(prev: unknown) =>
					updateGridItems(prev, validatedUserItem, orderBy, order)
			);
			refetch();
			refetchUsersLookup();
		},
		onError: createOnErrorHandler(toast),
	});

	return {
		isEmailDuplicate,
		setIsEmailDuplicate,
		isUserCreating,
		updateUserStatusMutation,
		onUserCreate,
		onUserUpdate,
	};
};
