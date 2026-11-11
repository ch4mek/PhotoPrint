import { useQueryClient } from '@tanstack/react-query';
import { Formik } from 'formik';
import { useMemo, useState } from 'react';

import { Loader } from '@/components/Loader';
import { NotificationType } from '@/constants/notifications';
import { SignatureQueriesKeys } from '@/constants/queriesKeys';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { getTrimedString } from '@/helpers/getTrimedString';
import { getFullName } from '@/helpers/utils';
import { useGetSignature, useUpdateSignature } from '@/queries/signature';

import { MODSEN_URL } from './constants';
import { validationSchema } from './schema';
import { Signature } from './Signature';
import type { SignatureProps } from './Signature/types';
import { SignatureClearDialog } from './SignatureClearDialog';
import { SignatureControls } from './SignatureControls';
import { SignatureEditingPopUp } from './SignatureEditingPopUp';
import { SignatureWrapper } from './styles';

export function SignatureForm() {
	const toast = useToast();
	const queryClient = useQueryClient();
	const { lastName, firstName, position, email } = useAuth();
	const [isOpen, setIsOpen] = useState(false);
	const [isDialogOpen, setIsDialogOpen] = useState(false);
	const [submittedValues, setSubmittedValues] = useState<SignatureProps>();

	const handleClose = () => {
		setIsOpen(false);
	};

	const showToast = (message: string, type: NotificationType) =>
		toast.open(createToast(type, message));

	const {
		data: signatureData,
		isLoading,
		isFetching,
	} = useGetSignature(showToast);
	const { mutate } = useUpdateSignature(showToast, {
		onSuccess: (values) => {
			queryClient.setQueryData([SignatureQueriesKeys.signature], () => ({
				...values,
			}));
		},
	});

	const isDataValid = Boolean(
		signatureData &&
			signatureData.fullName &&
			signatureData.position &&
			signatureData.phone
	);

	const getFormattedSubmitValues = (values: SignatureProps) =>
		Object.keys(values).reduce(
			(result, field) => ({
				...result,
				[field]: getTrimedString(values[field]),
			}),
			{}
		);

	const handleSubmit = (values: SignatureProps) => {
		const formattedValues = getFormattedSubmitValues(values) as SignatureProps;

		setSubmittedValues(() => ({ ...formattedValues }));

		mutate(formattedValues);
		setIsOpen(false);
	};

	const resetedInitialValues = useMemo(
		() => ({
			fullName: getFullName(firstName, lastName),
			position: '',
			phone: '',
			email,
			website: MODSEN_URL,
		}),
		[lastName, firstName]
	);

	const initValues: SignatureProps = useMemo(
		() => ({
			fullName: signatureData?.fullName || getFullName(firstName, lastName),
			position: signatureData?.position || '',
			phone: signatureData?.phone || '',
			email,
			website: MODSEN_URL,
		}),
		[lastName, firstName, email, position, signatureData]
	);

	return (
		<Loader isLoading={isLoading || isFetching} size="small">
			<Formik
				enableReinitialize
				initialValues={initValues}
				validationSchema={validationSchema}
				validateOnBlur={false}
				validateOnMount
				onSubmit={handleSubmit}
			>
				{({ values, resetForm, submitForm, validateForm, setErrors }) => {
					const handleCloseWithReset = () => {
						resetForm();
						handleClose();
					};

					const handleOpen = () => {
						setIsOpen(true);
						setErrors({});
					};

					return (
						<>
							{isOpen && (
								<SignatureEditingPopUp
									onClose={handleCloseWithReset}
									onApply={submitForm}
								/>
							)}
							<SignatureWrapper>
								<Signature {...(submittedValues || initValues)} />
								<SignatureControls
									{...{
										values,
										setIsDialogOpen,
										isValid: isDataValid,
										showToast,
										showForm: handleOpen,
										setErrors,
									}}
								/>
							</SignatureWrapper>
							{isDialogOpen && (
								<SignatureClearDialog
									{...{
										isDialogOpen,
										setIsDialogOpen,
										resetForm,
										showToast,
										initValues: resetedInitialValues,
										validateForm,
										setSubmittedValues,
									}}
								/>
							)}
						</>
					);
				}}
			</Formik>
		</Loader>
	);
}
