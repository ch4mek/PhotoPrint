import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo } from 'react';

import { SkillName } from '../constants';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import {
	ITEM_ADDED_POSTFIX,
	ITEM_UPDATED_POSTFIX,
	NotificationType,
} from '@/constants/notifications';
import { SettingsSkillsQueriesKeys } from '@/constants/queriesKeys';
import { ToastContextValues } from '@/context/types';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createToast } from '@/helpers/createToast';
import { useTrimInfiniteQueryOnUnmount } from '@/hooks/useTrimInfiniteQueryOnUnmount';
import { useCreateHardSkill } from '@/queries/settings/useCreateHardSkill';
import { useDeleteHardSkill } from '@/queries/settings/useDeleteHardSkill';
import { useHardSkillsInfinite } from '@/queries/settings/useHardSkillsInfinite';
import { useUpdateHardSkill } from '@/queries/settings/useUpdateHardSkill';
import { HardSkill } from '@/types/settings';

interface UseHardSkillsManagementProps {
	selectedDepartmentId: string | null;
	selectedSpecializationId?: number;
	toast: ToastContextValues;
}

export function useHardSkillsManagement({
	selectedDepartmentId,
	selectedSpecializationId,
	toast,
}: UseHardSkillsManagementProps) {
	const queryClient = useQueryClient();

	const {
		data,
		isLoading,
		isFetching,
		fetchNextPage,
		hasNextPage,
		isFetchingNextPage,
	} = useHardSkillsInfinite({
		departmentId: selectedDepartmentId,
		specializationId: selectedSpecializationId,
	});

	useTrimInfiniteQueryOnUnmount([
		SettingsSkillsQueriesKeys.hardSkills,
		selectedDepartmentId,
		selectedSpecializationId,
	]);

	const hardSkills = useMemo(
		() => data?.pages.flatMap((page) => page.items) ?? [],
		[data]
	);

	const total = useMemo(() => data?.pages[0]?.total ?? 0, [data]);

	const deleteHardSkillMutation = useDeleteHardSkill(toast);

	const deleteHardSkill = useCallback(
		async (skill: HardSkill) => {
			await deleteHardSkillMutation.mutateAsync(skill);

			queryClient.invalidateQueries({
				queryKey: [SettingsSkillsQueriesKeys.hardSkills],
			});
		},
		[deleteHardSkillMutation, queryClient]
	);

	const { mutateAsync: createHardSkillMutateAsync } = useCreateHardSkill({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					`${capitalizeString(SkillName.HardSkill)} ${ITEM_ADDED_POSTFIX}`
				)
			);

			queryClient.invalidateQueries({
				queryKey: [SettingsSkillsQueriesKeys.hardSkills],
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

	const { mutateAsync: updateHardSkillMutateAsync } = useUpdateHardSkill({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					`${capitalizeString(SkillName.HardSkill)} ${ITEM_UPDATED_POSTFIX}`
				)
			);

			queryClient.invalidateQueries({
				queryKey: [SettingsSkillsQueriesKeys.hardSkills],
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

	return {
		hardSkills,
		total,
		isLoading,
		isFetching,
		fetchNextPage,
		hasNextPage,
		isFetchingNextPage,

		createHardSkill: createHardSkillMutateAsync,
		updateHardSkill: updateHardSkillMutateAsync,
		deleteHardSkill,
	};
}
