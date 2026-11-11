import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useCreateColorPicker } from '@/queries/useCreateColorPicker';
import { useDeleteColorPicker } from '@/queries/useDeleteColorPicker';
import { GET_USER_COLORS_QUERY_KEY } from '@/queries/useGetColorPicker';

export const useColorPickerMutations = () => {
	const queryClient = useQueryClient();
	const toast = useToast();

	const handleSuccess = useCallback(() => {
		queryClient.invalidateQueries({ queryKey: [GET_USER_COLORS_QUERY_KEY] });
	}, [queryClient]);

	const createColorPickerMutation = useCreateColorPicker({
		onSuccess: handleSuccess,
		onError: createOnErrorHandler(toast),
	});

	const deleteColorPickerMutation = useDeleteColorPicker({
		onSuccess: handleSuccess,
		onError: createOnErrorHandler(toast),
	});

	const createColorMutation = (color: string) =>
		createColorPickerMutation.mutateAsync({ color });
	const deleteColorMutation = (color: string) =>
		deleteColorPickerMutation.mutateAsync({ color });

	return {
		createColorMutation,
		deleteColorMutation,
	};
};
