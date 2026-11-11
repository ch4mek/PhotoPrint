import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useGetAbsences } from '@/queries/absences/useGetAbsences';

export const useGetAbsencesData = () => {
	const toast = useToast();

	const { data, isLoading } = useGetAbsences({
		onError: createOnErrorHandler(toast),
	});

	return {
		absenceItems: data || [],
		isLoading,
	};
};
