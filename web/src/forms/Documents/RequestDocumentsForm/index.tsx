import { useTheme } from '@emotion/react';
import { useMemo, useState } from 'react';

import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useResize } from '@/hooks/useResize';
import { useGetUserAbsences } from '@/queries/absences/useGetUserAbsences';
import { AbsenceType, UserAbsenceItem } from '@/types/absences';

import {
	MAXIMUM_REQUEST_DOCUMENT_COUNT,
	requestDocumentExtensions,
} from './constants';
import { useRequestDocuments } from './hooks/useRequestDocuments';
import { RequestDocumentsFormContent } from './RequestDocumentsFormContent';
import { RequestDocumentsFormProps } from './types';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';

export function RequestDocumentsForm({
	handleClose,
	handleSubmit,
	userId,
	isRequestCreating = false,
}: RequestDocumentsFormProps) {
	const theme = useTheme();
	const toast = useToast();
	const [, windowHeight] = useResize();

	const { data: absences, isLoading: isLoadingUserAbsences } =
		useGetUserAbsences(
			userId!,
			{
				enabled: !!userId,
				onError: createOnErrorHandler(toast),
			},
			true
		);

	const documentHandlers = useRequestDocuments({
		extensions: requestDocumentExtensions,
		maxCount: MAXIMUM_REQUEST_DOCUMENT_COUNT,
	});

	const [selectedDocument, setSelectedDocument] =
		useState<UserAbsenceItem | null>(null);

	const documentTypeOptions = useMemo(() => {
		if (!absences) {
			return [];
		}

		return absences
			.filter((absence) => absence.type === AbsenceType.Documents)
			.map(({ name, inquiryTemplateId }) => ({
				id: inquiryTemplateId,
				name,
			}));
	}, [absences]);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const isShowLoader = isLoadingUserAbsences;

	return (
		<RequestDocumentsFormContent
			userId={userId}
			isRequestCreating={isRequestCreating}
			isShowLoader={isShowLoader}
			absences={absences}
			selectedDocument={selectedDocument}
			setSelectedDocument={setSelectedDocument}
			documentTypeOptions={documentTypeOptions}
			documentHandlers={documentHandlers}
			popUpStylesConfig={popUpStylesConfig}
			handleClose={handleClose}
			handleSubmit={handleSubmit}
		/>
	);
}
