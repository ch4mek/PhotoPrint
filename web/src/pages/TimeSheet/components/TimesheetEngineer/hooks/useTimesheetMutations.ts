import { MutateOptions } from '@tanstack/react-query';
import { useCallback, useMemo } from 'react';

import { ToastContextValues } from '@/context/types';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useUpdateTimesheetTaskName } from '@/queries/timesheet';
import { UpdateTaskNameVariables } from '@/queries/timesheet/types';

export const useTimesheetMutations = (
	toast: ToastContextValues,
	userId: number
) => {
	const updateTaskNameMutation = useUpdateTimesheetTaskName();

	const updateTaskName = useCallback(
		(
			queryParams: Omit<UpdateTaskNameVariables, 'userId'>,
			options?:
				| MutateOptions<unknown, unknown, UpdateTaskNameVariables, unknown>
				| undefined
		) => {
			const requestProps = {
				...queryParams,
				userId,
			};
			updateTaskNameMutation
				.mutateAsync(requestProps, options)
				.then((response) => {
					if (options?.onSuccess) {
						options.onSuccess(response, requestProps, {});
					}

					return response;
				})
				.catch((error) => {
					if (options?.onError) {
						options.onError(error, requestProps, {});
					}

					createOnErrorHandler(toast)(error);

					return error;
				})
				.finally(() => {
					if (options?.onSettled) {
						options.onSettled(undefined, null, requestProps, {});
					}
				});
		},
		[]
	);

	return useMemo(
		() => ({
			updateTaskName,
		}),
		[updateTaskName]
	);
};
