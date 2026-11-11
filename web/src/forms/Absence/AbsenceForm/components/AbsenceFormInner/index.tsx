import { useEffect } from 'react';

import { AbsenceTabs } from '../../constants';
import { ApprovalsTab } from '../ApprovalsTab';
import { DetailsTab } from '../DetailsTab';

import { AbsenceFormInnerProps } from './types';

const { Details, Approvals } = AbsenceTabs;

export function AbsenceFormInner({
	formikProps,
	setIsDetailsTabValid,
	setIsNameExists,
	checkIsNameExist,
	dataId,
	dataHasAnyTransactions,
	absenceTypesOptions,
	activityTypesOptions,
	accrualTypeOptions,
	activeTab,
	submitFormButtonRef,
}: AbsenceFormInnerProps) {
	const {
		values,
		errors,
		touched,
		setFieldValue,
		setFieldTouched,
		validateField,
		validateForm,
	} = formikProps;

	useEffect(() => {
		const checkDetailsValidity = async () => {
			const formErrors = await validateForm(values);
			const hasDetailsErrors =
				formErrors.details && Object.keys(formErrors.details).length > 0;

			setIsDetailsTabValid(!hasDetailsErrors);
		};

		checkDetailsValidity();
	}, [values.details, validateForm]);

	useEffect(() => {
		const nameExists = checkIsNameExist(
			values.details.name,
			dataId ?? undefined
		);
		setIsNameExists(nameExists);
	}, [values.details.name, dataId]);

	return (
		<>
			{activeTab === Details && (
				<DetailsTab
					values={values.details}
					errors={errors.details}
					touched={touched.details}
					setFieldValue={setFieldValue}
					setFieldTouched={setFieldTouched}
					validateField={validateField}
					absenceTypesOptions={absenceTypesOptions}
					activityTypesOptions={activityTypesOptions}
					accrualTypeOptions={accrualTypeOptions}
					disableAutoselectFields={!!dataHasAnyTransactions}
				/>
			)}
			{activeTab === Approvals && (
				<ApprovalsTab
					ref={submitFormButtonRef}
					values={values.approvals}
					errors={errors.approvals}
					touched={touched.approvals}
					setFieldValue={setFieldValue}
				/>
			)}
		</>
	);
}
