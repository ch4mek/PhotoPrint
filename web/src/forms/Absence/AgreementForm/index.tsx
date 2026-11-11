import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { spaces } from '@mms/mms-ui-library/theme';
import { useCallback, useMemo } from 'react';

import { wrapperStyles } from '../AbsenceForm/components/DetailsTab/constants';
import { CheckboxLabel } from '../AbsenceForm/components/DetailsTab/styles';
import { FieldWithDescription } from '../AbsenceForm/components/FieldWithDescription';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import {
	Agreement,
	AgreementDetailsFields,
	AgreementItem,
} from '@/types/absences';

import {
	AGREEMENT_ENTITY_NAME,
	agreementsFieldData,
	initialFormValues,
	FORM_NAME,
} from './constants';
import { validationSchema } from './schema';
import { AgreementFormWrapper } from './styles';
import { InitialFormValues } from './types';
import {
	convertDataToSubmit,
	convertUpdatedDataToSubmit,
} from './utils/convertDataToSubmit';
import { convertInitialDataToForm } from './utils/convertInitialDataToForm';

export function AgreementForm({
	handleAddAgreementSubmit,
	handleUpdateAgreementSubmit,
	handleClose,
	agreementItem,
	isLoading,
}: {
	handleClose: VoidFunction;
	handleAddAgreementSubmit: (values: Agreement) => Promise<void>;
	agreementItem: { itemId: string | number; item?: AgreementItem } | undefined;
	handleUpdateAgreementSubmit: (values: AgreementItem) => Promise<void>;
	isLoading: boolean;
}) {
	const { itemId: inquiryTemplateId, item: agreement } = agreementItem ?? {};

	const isEditForm = Boolean(agreement?.id);

	const handleSubmit = useCallback(
		async (values: InitialFormValues) => {
			if (!inquiryTemplateId) {
				return;
			}

			if (isEditForm && agreement) {
				await handleUpdateAgreementSubmit(
					convertUpdatedDataToSubmit(values, agreement.id)
				);
			} else {
				await handleAddAgreementSubmit(
					convertDataToSubmit(values, +inquiryTemplateId)
				);
			}
		},
		[handleAddAgreementSubmit, inquiryTemplateId, isEditForm, agreement]
	);

	const initialData = useMemo(() => {
		if (!isEditForm || !agreement) {
			return initialFormValues;
		}

		return convertInitialDataToForm(agreement);
	}, [agreement, isEditForm]);

	return (
		<FormikWithBackup
			initialValues={initialData}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			enableReinitialize
			validateOnBlur={false}
			validateOnChange
			name={FORM_NAME}
			id={`${inquiryTemplateId}_${agreement?.id}`}
		>
			{({ setFieldValue, values, submitForm, handleOutsideClose }) => {
				const handleCheckBoxClick = (field: AgreementDetailsFields) => () => {
					setFieldValue(field, !values[field]);
				};

				return (
					<PopUp
						type="form"
						showLoader={isLoading}
						title={
							isEditForm
								? getEditingPopUpTitle(AGREEMENT_ENTITY_NAME)
								: getAddingPopUpTitle(AGREEMENT_ENTITY_NAME)
						}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: handleClose },
						}}
					>
						<AgreementFormWrapper>
							<TextField
								{...agreementsFieldData.name}
								onChange={setFieldValue}
								isRequired
							/>

							<TextField
								{...agreementsFieldData.daysPerContractYear}
								onChange={setFieldValue}
								disabled={!!agreement?.hasAnyTransactions}
								isRequired
							/>

							<Checkbox
								key="isDefault"
								{...agreementsFieldData.isDefault}
								{...{ wrapperStyles }}
								checked={!!values.isDefault}
								onClick={handleCheckBoxClick('isDefault')}
								label={
									<FieldWithDescription
										{...{
											gapSize: spaces.xs,
											description:
												agreementsFieldData.isDefault.description || '',
										}}
									>
										<CheckboxLabel variant="m-400">
											{agreementsFieldData.isDefault.label}
										</CheckboxLabel>
									</FieldWithDescription>
								}
							/>
						</AgreementFormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
