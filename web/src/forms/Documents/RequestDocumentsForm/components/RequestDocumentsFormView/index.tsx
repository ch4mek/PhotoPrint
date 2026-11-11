import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useFormikContext } from 'formik';
import { useCallback } from 'react';

import { REQUEST_DOCUMENTS_POPUP_TITLE } from '../../constants';

import { AttachFilesGroupWithTypes } from '@/components/AttachFilesGroupWithTypes';
import { PositiveActionButton } from '@/components/PositiveActionButton';
import { TextArea } from '@/fields/TextArea';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';

import { requestFieldData, TEXTAREA_MIN_ROWS } from './constants';
import { FormWrapper, RequestFieldGroupWrapper } from './styles';
import { RequestDocumentsFormViewProps } from './types';

export function RequestDocumentsFormView({
	values,
	errors,
	touched,
	isShowLoader,
	popUpStylesConfig,
	documentHandlers,
	documentTypeOptions,
	handleDocumentTypeChange,
	handleClose,
	handleOutsideClose,
	handlePositiveControl,
}: RequestDocumentsFormViewProps) {
	const { setFieldValue } = useFormikContext<{
		documentType: { id: string | number; name: string } | null;
		files: { fileId: string | number; fileName: string; file: File }[];
		notes: string | null;
	}>();

	const handleAttachedFileChange = useCallback(
		() => (e: React.ChangeEvent<HTMLInputElement>) => {
			const newFiles = documentHandlers.handleFileChange(
				e,
				values.files.length
			);

			if (newFiles) {
				setFieldValue('files', [...values.files, ...newFiles]);
			}

			return newFiles;
		},
		[documentHandlers, values.files, setFieldValue]
	);

	const handleAttachedFileRemove = useCallback(
		(fileId: string | number) => () => {
			const updatedFiles = values.files.filter(
				({ fileId: currentFileId }) => currentFileId !== fileId
			);
			setFieldValue('files', updatedFiles);
			documentHandlers.handleFileRemove(fileId);
		},
		[values.files, setFieldValue, documentHandlers]
	);

	return (
		<PopUp
			type="form"
			showLoader={isShowLoader}
			title={REQUEST_DOCUMENTS_POPUP_TITLE}
			onClose={handleClose}
			onShadowClick={handleOutsideClose}
			controls={{
				positiveControl: {
					customControl: (
						<PositiveActionButton
							onClick={isShowLoader ? undefined : handlePositiveControl}
						/>
					),
				},
				negativeControl: { onClick: handleClose },
			}}
			stylesConfig={popUpStylesConfig}
		>
			<FormWrapper>
				<RequestFieldGroupWrapper>
					<Autoselect
						{...requestFieldData.documentType}
						value={values.documentType}
						onChange={handleDocumentTypeChange}
						options={documentTypeOptions}
						valueField="name"
						error={
							Boolean(errors.documentType) && Boolean(touched.documentType)
						}
						errorText={
							typeof errors.documentType === 'string'
								? errors.documentType
								: undefined
						}
						getKey={(option: { id: string | number }) => option.id}
						checkIfOptionEqualToValue={checkIfOptionsEqualById}
						shouldBeClosedOnOutsideScroll={false}
						disableClearable
						isRequired
					/>
				</RequestFieldGroupWrapper>

				<TextArea
					{...requestFieldData.notes}
					size="fullWidth"
					minRows={TEXTAREA_MIN_ROWS}
				/>

				<AttachFilesGroupWithTypes
					files={values.files}
					onAttachedFileChange={handleAttachedFileChange}
					onAttachedFileRemove={handleAttachedFileRemove}
					isAttachMultiple
				/>
			</FormWrapper>
		</PopUp>
	);
}
