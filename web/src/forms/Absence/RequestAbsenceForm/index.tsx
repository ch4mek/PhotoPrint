import { useTheme } from '@emotion/react';
import { useCallback, useMemo } from 'react';

import { useToast } from '@/context/Toast';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { convertDataToHolidays } from '@/helpers/absences';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useResize } from '@/hooks/useResize';
import { useGetUserAbsences } from '@/queries/absences/useGetUserAbsences';
import { useGetHolidaysByUserCountry } from '@/queries/calendar/getHolidaysByUserCountry';
import { useUploadFiles } from '@/queries/files/useUploadFiles';
import { useGetRequestSelectedDays } from '@/queries/inquiries/useGetRequestSelectedDays';
import { TrackingUnitType } from '@/types/absences';

import { RequestAbsenceFormView } from './components/RequestAbsenceFormView';
import {
	initialData,
	MAXIMUM_REQUEST_DOCUMENT_COUNT,
	REQUEST_POPUP_TITLE,
	requestDocumentExtensions,
} from './constants';
import { useAbsenceSelection } from './hooks/useAbsenceSelection';
import { useRequestDocuments } from './hooks/useRequestDocuments';
import { getValidationSchema } from './schema';
import { InitialDataValues, RequestAbsenceFormProps } from './types';
import { getFilesToUpload } from './utils/getFilesToUpload';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';

export function RequestAbsenceForm({
	handleClose,
	handleSubmit,
	initialAbsenceTypeId,
	userId,
	locationId,
	isRequestCreating = false,
	formType,
}: RequestAbsenceFormProps) {
	const theme = useTheme();
	const toast = useToast();
	const [, windowHeight] = useResize();

	const { data: absences, isLoading: isLoadingUserAbsences } =
		useGetUserAbsences(
			userId as number,
			{
				enabled: !!userId,
				onError: createOnErrorHandler(toast),
			},
			true
		);

	const { data: holidaysData, isLoading: isLoadingHolidays } =
		useGetHolidaysByUserCountry(locationId, {
			enabled: !!locationId,
		});

	const { data: selectedDays, getRequestSelectedDays } =
		useGetRequestSelectedDays();

	const { mutateAsync: uploadDocuments, isPending: isUploadDocumentsPending } =
		useUploadFiles();

	const {
		selectedAbsence,
		absenceNamesOptions,
		initialAbsenceName,
		setSelectedAbsence,
	} = useAbsenceSelection({
		absences,
		initialAbsenceTypeId,
	});

	const documentHandlers = useRequestDocuments({
		extensions: requestDocumentExtensions,
		maxCount: MAXIMUM_REQUEST_DOCUMENT_COUNT,
	});

	const holidays = useMemo(
		() => convertDataToHolidays(holidaysData),
		[holidaysData]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const validationSchema = useMemo(
		() =>
			getValidationSchema(
				selectedAbsence?.trackingUnitType === TrackingUnitType.Days
			),
		[selectedAbsence]
	);

	const initialFormData = useMemo(
		() => ({
			...initialData,
			absenceName: initialAbsenceName,
		}),
		[initialAbsenceName]
	);

	const handleSubmitForm = useCallback(
		async (value: InitialDataValues) => {
			const documentsIds = documentHandlers.documents
				? await uploadDocuments(getFilesToUpload(documentHandlers.documents))
				: null;

			return handleSubmit({
				...value,
				hours:
					selectedAbsence?.trackingUnitType === TrackingUnitType.Hours
						? value.hours
						: null,
				inquiryTemplateId: selectedAbsence?.inquiryTemplateId || 0,
				agreementTemplateId: selectedAbsence?.agreementTemplateId,
				documentsIds: documentsIds ?? null,
			});
		},
		[
			documentHandlers.documents,
			uploadDocuments,
			handleSubmit,
			selectedAbsence?.trackingUnitType,
			selectedAbsence?.inquiryTemplateId,
			selectedAbsence?.agreementTemplateId,
		]
	);

	const isDisableForm = !absenceNamesOptions.length;
	const isShowLoader =
		isLoadingUserAbsences ||
		isLoadingHolidays ||
		isUploadDocumentsPending ||
		isRequestCreating;

	return (
		<FormikWithBackup
			initialValues={initialFormData}
			validationSchema={validationSchema}
			onSubmit={handleSubmitForm}
			onClose={handleClose}
			validateOnBlur={false}
			name={REQUEST_POPUP_TITLE}
			id={`${userId}|${formType}|${initialAbsenceTypeId}`}
			enableReinitialize
			validateOnChange
		>
			{(formikProps) => (
				<RequestAbsenceFormView
					{...{
						formikProps,
						userId,
						selectedAbsence,
						absences,
						absenceNamesOptions,
						holidays,
						isDisableForm,
						isShowLoader,
						popUpStylesConfig,
						documentHandlers,
						selectedDays,
						handleClose,
						getRequestSelectedDays,
						setSelectedAbsence,
					}}
				/>
			)}
		</FormikWithBackup>
	);
}
