import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo } from 'react';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import {
	NotificationType,
	PERFORMANCE_REVIEW_HISTORY_UPDATED,
} from '@/constants/notifications';
import { performanceReviewQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { getDateAsResponseType } from '@/helpers/booking';
import { createToast } from '@/helpers/createToast';
import dayjs from '@/lib/dayjs';
import { useCreatePerformanceReview } from '@/queries/performanceReview';

import { AddingPerformanceReviewFormContent } from './components/AddingPerformanceReviewFormContent';
import { FORMIK_INITIAL_VALUES, POPUP_TITLE } from './constants';
import { validationSchema } from './schema';
import { AddingPerformanceReviewFormProps, FormValues } from './types';

export function AddingPerformanceReviewForm({
	onClose,
	initialValues,
	refetch,
	isInitialValueDisabled = true,
}: AddingPerformanceReviewFormProps) {
	const toast = useToast();
	const queryClient = useQueryClient();

	const createPerformanceReview = useCreatePerformanceReview({
		onSuccess: () => {
			onClose();
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					PERFORMANCE_REVIEW_HISTORY_UPDATED
				)
			);

			refetch?.();
			queryClient.invalidateQueries({
				queryKey: [performanceReviewQueriesKeys.getAllPerformanceReviews],
			});
		},

		onError(error) {
			toast.open(
				createToast(
					NotificationType.ERROR,
					error.response?.data?.Message || SOMETHING_WENT_WRONG
				)
			);
		},
	});

	const handleSubmit = useCallback(async ({ date, user }: FormValues) => {
		if (date && user?.id) {
			const convertedDate = getDateAsResponseType(dayjs(date));

			await createPerformanceReview.mutateAsync({
				date: convertedDate,
				userId: user.id,
			});
		}
	}, []);

	const initialPerformanceReviewValues = useMemo(
		() => ({
			...FORMIK_INITIAL_VALUES,
			user: initialValues?.userData || null,
		}),
		[initialValues]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			validateOnChange
			validateOnBlur={false}
			initialValues={initialPerformanceReviewValues}
			onSubmit={handleSubmit}
			name={POPUP_TITLE}
			id={initialValues?.userData?.id}
			{...{ validationSchema, onClose }}
		>
			{(formikProps) => (
				<AddingPerformanceReviewFormContent
					{...{ formikProps, isInitialValueDisabled, initialValues, onClose }}
				/>
			)}
		</FormikWithBackup>
	);
}
