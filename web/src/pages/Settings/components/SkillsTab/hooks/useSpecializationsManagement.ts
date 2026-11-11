import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo } from 'react';

import { SkillName } from '../constants';

import { SPECIALIZATION_WITHOUT_HARD_SKILLS } from '@/constants/errors';
import {
	ASSIGNED_SPECIALIZATION_DELETING_MESSAGE,
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
import { useCreateSpecialization } from '@/queries/settings/useCreateSpecialization';
import { useDeleteSpecialization } from '@/queries/settings/useDeleteSpecialization';
import { useGetSpecializationById } from '@/queries/settings/useGetSpecializationById';
import { useSpecializationsInfinite } from '@/queries/settings/useSpecializationsInfinite';
import { useUpdateSpecialization } from '@/queries/settings/useUpdateSpecialization';
import { DetailedSpecialization, Specialization } from '@/types/settings';

interface UseSpecializationsManagementProps {
	selectedDepartmentId: string | null;
	selectedSpecialization: Specialization | null;
	toast: ToastContextValues;
	handleSpecializationItemClick: (specialization: Specialization) => void;
}

export function useSpecializationsManagement({
	selectedDepartmentId,
	selectedSpecialization,
	toast,
	handleSpecializationItemClick,
}: UseSpecializationsManagementProps) {
	const queryClient = useQueryClient();

	const {
		data,
		isLoading,
		isFetching,
		fetchNextPage,
		hasNextPage,
		isFetchingNextPage,
	} = useSpecializationsInfinite({
		departmentId: selectedDepartmentId,
	});

	useTrimInfiniteQueryOnUnmount([
		SettingsSkillsQueriesKeys.specializations,
		selectedDepartmentId,
	]);

	const specializations = useMemo(
		() => data?.pages.flatMap((page) => page.items) ?? [],
		[data]
	);

	const total = useMemo(() => data?.pages[0]?.total ?? 0, [data]);

	const { mutateAsync: createSpecializationMutateAsync } =
		useCreateSpecialization({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						`${capitalizeString(
							SkillName.Specialization
						)} ${ITEM_ADDED_POSTFIX}`
					)
				);

				queryClient.invalidateQueries({
					queryKey: [SettingsSkillsQueriesKeys.hardSkills],
				});

				queryClient.invalidateQueries({
					queryKey: [SettingsSkillsQueriesKeys.specializations],
				});
			},
			onError: (error) => {
				if (error?.response?.data?.Message) {
					toast.open(
						createToast(NotificationType.ERROR, error.response.data.Message)
					);
				}
			},
		});

	const { mutateAsync: updateSpecializationMutateAsync } =
		useUpdateSpecialization({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						`${capitalizeString(
							SkillName.Specialization
						)} ${ITEM_UPDATED_POSTFIX}`
					)
				);

				queryClient.invalidateQueries({
					queryKey: [SettingsSkillsQueriesKeys.hardSkills],
				});

				queryClient.invalidateQueries({
					queryKey: [SettingsSkillsQueriesKeys.specializations],
				});
			},
			onError: (error) => {
				if (error?.response?.data?.Message) {
					const message = error.response.data.Message;

					if (message.includes(SPECIALIZATION_WITHOUT_HARD_SKILLS)) {
						toast.open(createToast(NotificationType.ERROR, message));

						return;
					}

					toast.open(createToast(NotificationType.ERROR, message));
				}
			},
		});

	const deleteSpecializationMutation = useDeleteSpecialization({
		onSuccess: ({ id: specIdToDelete, name, canBeUpdated }: Specialization) => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					`${capitalizeString(
						SkillName.Specialization
					)} ${ITEM_DELETED_POSTFIX}`
				)
			);

			if (selectedSpecialization?.id === specIdToDelete) {
				handleSpecializationItemClick({
					id: specIdToDelete,
					name,
					canBeUpdated,
				});
			} else {
				queryClient.invalidateQueries({
					queryKey: [SettingsSkillsQueriesKeys.hardSkills],
				});
			}

			queryClient.invalidateQueries({
				queryKey: [SettingsSkillsQueriesKeys.specializations],
			});
		},
		onError: ({ response }) => {
			if (response?.data?.Message) {
				toast.open(createToast(NotificationType.ERROR, response.data.Message));
			}
		},
	});

	const deleteSpecialization = deleteSpecializationMutation.mutateAsync;

	const getSpecializationByIdMutation = useGetSpecializationById({
		onSuccess: ({ departmentDtos }: DetailedSpecialization) => {
			const hasUserAssigned = departmentDtos.some(
				(department) => department.isUserFromDepartmentAssigned
			);

			if (hasUserAssigned) {
				toast.open(
					createToast(
						NotificationType.ERROR,
						ASSIGNED_SPECIALIZATION_DELETING_MESSAGE
					)
				);
			}
		},
		onError: ({ response }) => {
			if (response?.data?.Message) {
				toast.open(createToast(NotificationType.ERROR, response.data.Message));
			}
		},
	});

	const checkSpecializationBeforeDelete = useCallback(
		async (specializationId: string | number) => {
			try {
				const detailedSpec = await getSpecializationByIdMutation.mutateAsync(
					specializationId
				);

				const hasUserAssigned = detailedSpec.departmentDtos.some(
					(department) => department.isUserFromDepartmentAssigned
				);

				return !hasUserAssigned;
			} catch {
				return false;
			}
		},
		[getSpecializationByIdMutation]
	);

	return {
		specializations,
		total,
		isLoading,
		isFetching,
		fetchNextPage,
		hasNextPage,
		isFetchingNextPage,

		createSpecialization: createSpecializationMutateAsync,
		updateSpecialization: updateSpecializationMutateAsync,
		deleteSpecialization,
		checkSpecializationBeforeDelete,
	};
}
