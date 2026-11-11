import { useQueryClient } from '@tanstack/react-query';
import { useMemo } from 'react';

import { SkillName } from '../constants';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import {
	ITEM_ADDED_POSTFIX,
	ITEM_DELETED_POSTFIX,
	ITEM_UPDATED_POSTFIX,
	NotificationType,
} from '@/constants/notifications';
import { SettingsSkillsQueriesKeys } from '@/constants/queriesKeys';
import { ToastContextValues } from '@/context/types';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createToast } from '@/helpers/createToast';
import { useTrimInfiniteQueryOnUnmount } from '@/hooks/useTrimInfiniteQueryOnUnmount';
import { useCreateSoftSkill } from '@/queries/settings/useCreateSoftSkill';
import { useDeleteSoftSkill } from '@/queries/settings/useDeleteSoftSkill';
import { useSoftSkillsInfinite } from '@/queries/settings/useSoftSkillsInfinite';
import { useUpdateSoftSkill } from '@/queries/settings/useUpdateSoftSkill';

interface UseSoftSkillsManagementProps {
	toast: ToastContextValues;
}

export function useSoftSkillsManagement({
	toast,
}: UseSoftSkillsManagementProps) {
	const queryClient = useQueryClient();

	const {
		data,
		isLoading,
		isFetching,
		fetchNextPage,
		hasNextPage,
		isFetchingNextPage,
	} = useSoftSkillsInfinite();

	useTrimInfiniteQueryOnUnmount([SettingsSkillsQueriesKeys.softSkills]);

	const softSkills = useMemo(
		() => data?.pages.flatMap((page) => page.items) ?? [],
		[data]
	);

	const total = useMemo(() => data?.pages[0]?.total ?? 0, [data]);

	const { mutateAsync: createSoftSkillMutateAsync } = useCreateSoftSkill({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					`${capitalizeString(SkillName.SoftSkill)} ${ITEM_ADDED_POSTFIX}`
				)
			);

			queryClient.invalidateQueries({
				queryKey: [SettingsSkillsQueriesKeys.softSkills],
			});
		},
		onError: (error) => {
			if (error?.response?.data?.Message) {
				toast.open(
					createToast(NotificationType.ERROR, error.response.data.Message)
				);
			} else {
				toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));
			}
		},
	});

	const { mutateAsync: updateSoftSkillMutateAsync } = useUpdateSoftSkill({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					`${capitalizeString(SkillName.SoftSkill)} ${ITEM_UPDATED_POSTFIX}`
				)
			);

			queryClient.invalidateQueries({
				queryKey: [SettingsSkillsQueriesKeys.softSkills],
			});
		},
		onError: (error) => {
			if (error?.response?.data?.Message) {
				toast.open(
					createToast(NotificationType.ERROR, error.response.data.Message)
				);
			} else {
				toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));
			}
		},
	});

	const { mutateAsync: deleteSoftSkillMutateAsync } = useDeleteSoftSkill({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					`${capitalizeString(SkillName.SoftSkill)} ${ITEM_DELETED_POSTFIX}`
				)
			);

			queryClient.invalidateQueries({
				queryKey: [SettingsSkillsQueriesKeys.softSkills],
			});
		},
		onError: () => {
			toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));
		},
	});

	return {
		softSkills,
		total,
		isLoading,
		isFetching,
		fetchNextPage,
		hasNextPage,
		isFetchingNextPage,

		createSoftSkill: createSoftSkillMutateAsync,
		updateSoftSkill: updateSoftSkillMutateAsync,
		deleteSoftSkill: deleteSoftSkillMutateAsync,
	};
}
