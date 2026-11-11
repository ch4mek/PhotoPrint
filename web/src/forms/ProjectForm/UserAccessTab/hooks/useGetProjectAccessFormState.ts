import { QueryKey, useQueryClient } from '@tanstack/react-query';

import { DateFormat } from '@/constants/date';
import {
	NotificationType,
	PROJECT_HISTORY_UPDATED,
} from '@/constants/notifications';
import { ProjectsQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { ProjectAssignFormValues } from '@/forms/ProjectAccessAddingBase/types';
import { createToast } from '@/helpers/createToast';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import {
	UseGetUsersByProjectIdResponse,
	UsersByProjectId,
} from '@/queries/projects/types';
import { useAddUserProjectAssign } from '@/queries/useAddUserProjectAssign';
import { useEditUserProjectAssign } from '@/queries/useEditUserProjectAssign';

export const useGetProjectAccessFormState = (queryKeys: QueryKey) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { isOpen, id, handleOpen, handleClose } = useShowModal();

	const {
		mutateAsync: assignUserToProjectMutation,
		isPending: isAssignUserToProjectMutationLoading,
	} = useAddUserProjectAssign(toast, {
		onSuccess: (value) => {
			toast.open(
				createToast(NotificationType.SUCCESS, PROJECT_HISTORY_UPDATED)
			);
			queryClient.refetchQueries({ queryKey: [ProjectsQueriesKeys.projects] });
			queryClient.setQueryData<Partial<UseGetUsersByProjectIdResponse>>(
				queryKeys,
				(prevData) => ({
					...prevData,
					items: [value, ...(prevData?.items ?? [])] as UsersByProjectId[],
				})
			);

			handleClose();
		},
	});

	const {
		mutateAsync: editUserProjectAssignMutation,
		isPending: isEditUserProjectAssignLoading,
	} = useEditUserProjectAssign(toast, {
		onSuccess: (value) => {
			toast.open(
				createToast(NotificationType.SUCCESS, PROJECT_HISTORY_UPDATED)
			);
			queryClient.refetchQueries({ queryKey: [ProjectsQueriesKeys.projects] });

			queryClient.setQueryData<UseGetUsersByProjectIdResponse | undefined>(
				queryKeys,
				(prevData) => {
					if (!prevData) {
						return prevData;
					}

					const items = prevData.items.map((item: UsersByProjectId) =>
						item.id === value.id ? value : item
					) as UsersByProjectId[];

					return {
						...prevData,
						items,
					};
				}
			);

			handleClose();
		},
	});

	const handleFormSubmit = async (values: ProjectAssignFormValues) => {
		const {
			user,
			project,
			billability,
			fte,
			projectSeniority,
			startDate,
			endDate,
		} = values;

		if (user && project && projectSeniority) {
			if (id) {
				await editUserProjectAssignMutation({
					id: Number(id),
					userOnProjectFrom: dayjs(startDate).format(DateFormat.YYYY_MM_DD),
					userOnProjectTo: endDate
						? dayjs(endDate).format(DateFormat.YYYY_MM_DD)
						: endDate,
					isTimeOnProjectBillable: billability,
					fte: Number(fte),
					userOnProjectSeniorityLevelId: projectSeniority.id,
					userId: user.id,
					projectId: project.id,
				});

				return;
			}

			await assignUserToProjectMutation({
				userId: Number(user.id),
				isTimeOnProjectBillable: billability,
				fte: Number(fte),
				projectId: project.id,
				userOnProjectSeniorityLevelId: projectSeniority.id,
				userOnProjectFrom: dayjs(startDate).format(DateFormat.YYYY_MM_DD),
				userOnProjectTo: endDate
					? dayjs(endDate).format(DateFormat.YYYY_MM_DD)
					: endDate,
			});
		}
	};

	const isLoading =
		isAssignUserToProjectMutationLoading || isEditUserProjectAssignLoading;

	return {
		id,
		isOpen,
		isLoading,
		handleOpen,
		handleClose,
		handleFormSubmit,
	};
};
