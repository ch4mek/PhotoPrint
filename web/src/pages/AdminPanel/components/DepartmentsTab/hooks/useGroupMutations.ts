import { useCallback, useMemo } from 'react';

import { useToast } from '@/context/Toast';
import { useCreateGroup } from '@/queries/departments/useCreateGroup';
import { useDeleteGroup } from '@/queries/departments/useDeleteGroup';
import { useUpdateGroup } from '@/queries/departments/useUpdateGroup';
import { GroupMutationOptions, GroupMutationType } from '@/types/department';

export function useGroupMutations(
	mutationType: GroupMutationType,
	handleClose: VoidFunction,
	refetch: VoidFunction
): GroupMutationOptions {
	const toast = useToast();
	const {
		mutateAsync: createGroupMutation,
		isPending: isCreateGroupMutationLoading,
	} = useCreateGroup(toast);
	const {
		mutateAsync: updateGroupMutation,
		isPending: isUpdateGroupMutationLoading,
	} = useUpdateGroup(toast);
	const {
		mutateAsync: deleteGroupMutation,
		isPending: isDeleteGroupMutationLoading,
	} = useDeleteGroup(toast);

	const handleMutation = useCallback(
		<TValues, TResponse>(mutationFn: (values: TValues) => Promise<TResponse>) =>
			async (values: TValues) => {
				await mutationFn(values);

				handleClose();
				refetch();
			},
		[]
	);

	const groupMutationOptions = useMemo(() => {
		if (mutationType === GroupMutationType.Add) {
			return {
				isMutationLoading: isCreateGroupMutationLoading,
				handleMutation: handleMutation(createGroupMutation),
			};
		}

		if (mutationType === GroupMutationType.Edit) {
			return {
				isMutationLoading: isUpdateGroupMutationLoading,
				handleMutation: handleMutation(updateGroupMutation),
			};
		}

		return {
			isMutationLoading: isDeleteGroupMutationLoading,
			handleMutation: handleMutation(deleteGroupMutation),
		};
	}, [
		mutationType,
		isCreateGroupMutationLoading,
		isUpdateGroupMutationLoading,
		isDeleteGroupMutationLoading,
	]);

	return groupMutationOptions;
}
