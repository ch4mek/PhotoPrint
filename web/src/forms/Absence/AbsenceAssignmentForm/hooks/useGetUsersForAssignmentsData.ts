import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useGetUsersForAssignments } from '@/queries/absences/useGetUsersForAssignments';

export const useGetUsersForAssignmentsData = (
	absenceId: number | null,
	agreementId: number | null
) => {
	const toast = useToast();

	const { isLoading, refetch } = useGetUsersForAssignments(
		absenceId,
		agreementId,
		{
			enabled: false,
			onError: createOnErrorHandler(toast),
		}
	);

	return {
		isLoading,
		refetch,
	};
};
