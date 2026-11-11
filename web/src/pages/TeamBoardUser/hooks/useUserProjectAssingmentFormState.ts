import { QueryKey, useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import {
	NotificationType,
	PROJECT_HISTORY_UPDATED,
} from '@/constants/notifications';
import {
	payloadQueriesKeys,
	ratesQueriesKeys,
	TimesheetQueryKeys,
} from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { ProjectAssignFormValues } from '@/forms/ProjectAccessAddingBase/types';
import { createToast } from '@/helpers/createToast';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { EditUserProjectAssignParams } from '@/queries/types';
import { useAddUserProjectAssign } from '@/queries/useAddUserProjectAssign';
import { useEditUserProjectAssign } from '@/queries/useEditUserProjectAssign';

export const useUserProjectAssignmentFormState = (queryKeys: QueryKey) => {
	const { handleOpen, isOpen, handleClose, id } = useShowModal<number>();
	const toast = useToast();
	const queryClient = useQueryClient();

	const handleAssignmentMutationSuccess = () => {
		toast.open(createToast(NotificationType.SUCCESS, PROJECT_HISTORY_UPDATED));

		// TODO: [WORKAROUND] Timeout is a temporary fix due to backend race condition —
		// server doesn't return updated data immediately after mutation
		// Should be removed once backend ensures consistent response timing
		setTimeout(() => {
			queryClient.invalidateQueries({
				queryKey: [payloadQueriesKeys.getPayloadData],
			});
			queryClient.invalidateQueries({
				queryKey: [ratesQueriesKeys.getRates],
			});
			queryClient.invalidateQueries({
				queryKey: [ratesQueriesKeys.getRatesUsers],
			});
		}, 500);
		queryClient.invalidateQueries({
			queryKey: [TimesheetQueryKeys.projectsWithDisabledDays],
		});
		queryClient.invalidateQueries({
			queryKey: [TimesheetQueryKeys.userTimesheet],
		});
		queryClient.invalidateQueries({ queryKey: queryKeys });
		handleClose();
	};

	const {
		mutate: addUserProjectAssignmentMutation,
		isPending: isAddAssignmentLoading,
	} = useAddUserProjectAssign(toast, {
		onSuccess: handleAssignmentMutationSuccess,
	});

	const {
		mutate: editUserProjectAssignmentMutation,
		isPending: isEditAssignmentLoading,
	} = useEditUserProjectAssign(toast, {
		onSuccess: handleAssignmentMutationSuccess,
	});

	const handleProjectEditClick = useCallback(
		(projectId?: string | number) => () => {
			handleOpen(projectId as number);
		},
		[]
	);

	const handleFormSubmit = (values: ProjectAssignFormValues) => {
		const {
			user,
			billability,
			fte,
			project,
			startDate,
			endDate,
			projectSeniority,
		} = values;

		const submitFunction = id
			? (formValues: Omit<EditUserProjectAssignParams, 'id'>) =>
					editUserProjectAssignmentMutation({ id, ...formValues })
			: addUserProjectAssignmentMutation;

		if (user && project && projectSeniority) {
			submitFunction({
				userId: Number(user.id),
				isTimeOnProjectBillable: billability,
				fte: Number(fte),
				projectId: project.id,
				userOnProjectSeniorityLevelId: projectSeniority.id,
				userOnProjectFrom: dayjs(startDate).format(
					YYYY_MM_DD_DAYJS_DATE_FORMAT
				),
				userOnProjectTo: endDate
					? dayjs(endDate).format(YYYY_MM_DD_DAYJS_DATE_FORMAT)
					: endDate,
			});
		}
	};

	const showLoader = isAddAssignmentLoading || isEditAssignmentLoading;

	const handleAddClick = useCallback(() => {
		handleOpen();
	}, []);

	return {
		id,
		isOpen,
		showLoader,
		handleOpen,
		handleClose,
		handleFormSubmit,
		handleProjectEditClick,
		handleAddClick,
	};
};
