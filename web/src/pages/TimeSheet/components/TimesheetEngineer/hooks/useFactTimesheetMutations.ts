import { useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { useCallback } from 'react';

import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useOptimisticUpdate } from '@/pages/TimeSheet/components/TimesheetEngineer/hooks/useOptimisticUpdate';
import { FactTimesheet } from '@/pages/TimeSheet/components/TimesheetEngineer/types';
import { useDeleteTimesheet } from '@/queries/timesheet';
import {
	TimesheetDeleteResponse,
	TimesheetUpsertResponse,
	TrackedType,
} from '@/queries/timesheet/types';
import { useUpsertTimesheet } from '@/queries/timesheet/useUpsertTimesheet';
import { ServerResponseError } from '@/queries/types';

export const useFactTimesheetMutations = (
	cacheKey: string[],
	onSuccess: (data: TimesheetDeleteResponse | TimesheetUpsertResponse) => void,
	onError: (context: FactTimesheet) => void,
	onSettled: (error: unknown) => void
) => {
	const queryClient = useQueryClient();
	const { getPreviousData } = useOptimisticUpdate();
	const toast = useToast();

	const queryKey = [...cacheKey, TrackedType.Fact];

	const onMutate = async (newValue: FactTimesheet) => {
		const previousData = await getPreviousData<FactTimesheet>({
			queryKey,
			newValue,
		});

		return previousData;
	};

	const onErrorHandler = (
		error: AxiosError<ServerResponseError>,
		variables: FactTimesheet,
		context?: FactTimesheet
	) => {
		if (context) {
			queryClient.setQueryData(queryKey, context);

			if (onError) {
				onError(context);
			}
		}

		createOnErrorHandler(toast)(error);
	};

	const setDefaultFactCacheData = useCallback(
		(data: FactTimesheet) => {
			queryClient.setQueryData(queryKey, data);
		},
		[queryKey]
	);

	const onSuccessHandler = (
		data: TimesheetDeleteResponse | TimesheetUpsertResponse
	) => {
		if (onSuccess) {
			onSuccess(data);
		}
	};

	const onSettledHandler = async (
		data?: TimesheetDeleteResponse | TimesheetUpsertResponse,
		error?: unknown
	) => {
		await queryClient.invalidateQueries({ queryKey });

		if (onSettled) {
			onSettled(error);
		}
	};

	const { mutate: mutateFactUpsertTimesheet } =
		useUpsertTimesheet<FactTimesheet>(TrackedType.Fact, {
			onMutate,
			onError: onErrorHandler,
			onSettled: onSettledHandler,
			onSuccess: onSuccessHandler,
		});

	const { mutate: mutateFactDeleteTimesheet } =
		useDeleteTimesheet<FactTimesheet>(TrackedType.Fact, {
			onMutate,
			onError: onErrorHandler,
			onSettled: onSettledHandler,
			onSuccess: onSuccessHandler,
		});

	const getFactCacheData = useCallback(
		(): FactTimesheet | undefined => queryClient.getQueryData(queryKey),
		[queryKey]
	);

	return {
		mutateFactUpsertTimesheet,
		mutateFactDeleteTimesheet,
		setDefaultFactCacheData,
		getFactCacheData,
	};
};
