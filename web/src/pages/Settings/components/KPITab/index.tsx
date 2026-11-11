import { useQueryClient } from '@tanstack/react-query';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import { NotificationType } from '@/constants/notifications';
import { SettingsKPIColorsQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useGetKPIColors } from '@/queries/settings/kpiColors/useGetKPIColors';
import { useUpdateKPIColors } from '@/queries/settings/kpiColors/useUpdateKPIColors';

import { KPITabForm } from './components/KPITabForm';
import { KPI_VALUES_UPDATED_MESSAGE, FORM_NAME } from './constants';
import { validationSchema } from './schema';
import { KPIColorsFormValues } from './types';
import { getKPIFormValuesFromResponse } from './utils/getKPIFormValuesFromResponse';

export function KPITab() {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { data, isFetching } = useGetKPIColors();
	const {
		mutate,
		isPending: isUpdatingKPIColors,
		isError,
	} = useUpdateKPIColors({
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: [SettingsKPIColorsQueriesKeys.kpiColors],
			});
			toast.open(
				createToast(NotificationType.SUCCESS, KPI_VALUES_UPDATED_MESSAGE)
			);
		},
		onError: createOnErrorHandler(toast),
	});

	const handleFormSubmit = useCallback(
		(
			kpiColorValues: KPIColorsFormValues,
			{ setTouched }: FormikHelpers<KPIColorsFormValues>
		) => {
			mutate(
				Object.values(kpiColorValues).map(
					({ rightIntervalBorder, ...otherProperties }) => ({
						...otherProperties,
						rightIntervalBorder: Number(rightIntervalBorder),
					})
				)
			);
			setTouched({});
		},
		[mutate]
	);

	const initialValues = useMemo(
		() => getKPIFormValuesFromResponse(data),
		[data]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={validationSchema}
			onSubmit={handleFormSubmit}
			validateOnBlur={false}
			name={FORM_NAME}
		>
			{(formikProps) => (
				<KPITabForm
					{...{ formikProps, isFetching, isUpdatingKPIColors, isError }}
				/>
			)}
		</FormikWithBackup>
	);
}
