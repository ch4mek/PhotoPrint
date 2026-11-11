import { useEffect, useCallback } from 'react';

import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { useUploadFiles } from '@/queries/files/useUploadFiles';
import { Inquiry } from '@/queries/inquiries/types';
import { UserAbsenceItem } from '@/types/absences';

import { RequestDocumentsFormView } from './components/RequestDocumentsFormView';
import { REQUEST_DOCUMENTS_POPUP_TITLE, initialData } from './constants';
import { useRequestDocuments } from './hooks/useRequestDocuments';
import { getValidationSchema } from './schema';
import { InitialDataValues, RequestDocumentsValues } from './types';
import { findMatchingAbsence } from './utils/findMatchingAbsence';
import { getFilesToUpload } from './utils/getFilesToUpload';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';

interface RequestDocumentsFormContentProps {
	userId: number | undefined;
	isRequestCreating: boolean;
	isShowLoader: boolean;
	absences: UserAbsenceItem[] | undefined;
	selectedDocument: UserAbsenceItem | null;
	setSelectedDocument: (doc: UserAbsenceItem | null) => void;
	documentTypeOptions: { id: number; name: string }[];
	documentHandlers: ReturnType<typeof useRequestDocuments>;
	popUpStylesConfig: ReturnType<typeof getPopUpStylesConfig>;
	handleClose: VoidFunction;
	handleSubmit: (value: RequestDocumentsValues) => Promise<Inquiry>;
}

export function RequestDocumentsFormContent({
	userId,
	isRequestCreating,
	isShowLoader,
	absences,
	selectedDocument,
	setSelectedDocument,
	documentTypeOptions,
	documentHandlers,
	popUpStylesConfig,
	handleClose,
	handleSubmit,
}: RequestDocumentsFormContentProps) {
	const { mutateAsync: uploadDocuments, isPending: isUploadDocumentsPending } =
		useUploadFiles();

	const initialDocumentType = null;

	useEffect(() => {
		if (documentTypeOptions.length > 0 && !selectedDocument) {
			const matchingAbsence = findMatchingAbsence(
				absences,
				documentTypeOptions[0].id
			);

			if (matchingAbsence) {
				setSelectedDocument(matchingAbsence);
			}
		}
	}, [documentTypeOptions, absences, selectedDocument, setSelectedDocument]);

	const handleFormSubmit = useCallback(
		async (value: InitialDataValues) => {
			const documentsIds = documentHandlers.documents
				? await uploadDocuments(getFilesToUpload(documentHandlers.documents))
				: null;

			return handleSubmit({
				...value,
				inquiryTemplateId:
					selectedDocument?.inquiryTemplateId ??
					(Number(value.documentType?.id) || 0),
				agreementTemplateId: selectedDocument?.agreementTemplateId ?? null,
				documentsIds: documentsIds ?? null,
			});
		},
		[
			documentHandlers.documents,
			uploadDocuments,
			handleSubmit,
			selectedDocument?.inquiryTemplateId,
			selectedDocument?.agreementTemplateId,
		]
	);

	return (
		<FormikWithBackup
			initialValues={{
				...initialData,
				documentType: initialDocumentType,
			}}
			validationSchema={getValidationSchema()}
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={REQUEST_DOCUMENTS_POPUP_TITLE}
			id={userId}
			enableReinitialize
			validateOnChange
		>
			{({
				values,
				errors,
				touched,
				submitForm,
				setFieldValue,
				setFieldTouched,
				handleOutsideClose,
			}) => (
				<RequestDocumentsFormView
					{...{
						values,
						errors,
						touched,
						userId,
						popUpStylesConfig,
						documentHandlers,
						documentTypeOptions,
						handleClose,
						handleOutsideClose,
					}}
					isShowLoader={
						isShowLoader || isRequestCreating || isUploadDocumentsPending
					}
					handlePositiveControl={() => submitForm().catch(() => {})}
					handleDocumentTypeChange={(option) => {
						setFieldValue('documentType', option);
						setFieldTouched('documentType', true);

						if (option) {
							const matchingAbsence = findMatchingAbsence(absences, option.id);

							if (matchingAbsence) {
								setSelectedDocument(matchingAbsence);
							}
						}
					}}
				/>
			)}
		</FormikWithBackup>
	);
}
