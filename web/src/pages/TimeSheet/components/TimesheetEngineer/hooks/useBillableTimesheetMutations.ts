import { useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { useCallback } from 'react';

import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useOptimisticUpdate } from '@/pages/TimeSheet/components/TimesheetEngineer/hooks/useOptimisticUpdate';
import { BillableTimesheet } from '@/pages/TimeSheet/components/TimesheetEngineer/types';
import { useDeleteTimesheet } from '@/queries/timesheet';
import {
	TimesheetDeleteResponse,
	TimesheetUpsertResponse,
	TrackedType,
} from '@/queries/timesheet/types';
import { useUpsertTimesheet } from '@/queries/timesheet/useUpsertTimesheet';
import { ServerResponseError } from '@/queries/types';

export const useBillableTimesheetMutations = (
	cacheKey: string[],
	onSuccess: (data: TimesheetDeleteResponse | TimesheetUpsertResponse) => void,
	onError: (context: BillableTimesheet) => void,
	onSettled: VoidFunction
) => {
	const queryClient = useQueryClient();
	const { getPreviousData } = useOptimisticUpdate();
	const toast = useToast();

	const queryKey = [...cacheKey, TrackedType.Billable];

	const onMutate = async (newValue: BillableTimesheet) => {
		const previousData = await getPreviousData<BillableTimesheet>({
			queryKey,
			newValue,
		});

		return previousData;
	};

	const onErrorHandler = (
		error: AxiosError<ServerResponseError>,
		variables: BillableTimesheet,
		context?: BillableTimesheet
	) => {
		if (context) {
			queryClient.setQueryData(queryKey, context);

			if (onError) {
				onError(context);
			}
		}

		createOnErrorHandler(toast)(error);
	};

	const setDefaultBillableCacheData = useCallback(
		(data: BillableTimesheet) => {
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

	const onSettledHandler = async () => {
		await queryClient.invalidateQueries({ queryKey });

		if (onSettled) {
			onSettled();
		}
	};

	const { mutate: mutateBillableUpsertTimesheet } =
		useUpsertTimesheet<BillableTimesheet>(TrackedType.Billable, {
			onMutate,
			onError: onErrorHandler,
			onSettled: onSettledHandler,
			onSuccess: onSuccessHandler,
		});

	const { mutate: mutateBillableDeleteTimesheet } =
		useDeleteTimesheet<BillableTimesheet>(TrackedType.Billable, {
			onMutate,
			onError: onErrorHandler,
			onSettled: onSettledHandler,
			onSuccess: onSuccessHandler,
		});

	const getBillableCacheData = useCallback(
		(): BillableTimesheet | undefined => queryClient.getQueryData(queryKey),
		[queryKey]
	);

	return {
		mutateBillableUpsertTimesheet,
		mutateBillableDeleteTimesheet,
		setDefaultBillableCacheData,
		getBillableCacheData,
	};
};
