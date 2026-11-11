import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Formik, FormikProps, FormikValues } from 'formik';
import { useEffect, useMemo, useRef, useState } from 'react';

import { FormWithSaveExit } from '../FormWithSaveExit';

import { zIndexes } from '@/constants/zIndexes';
import { useAuth } from '@/context/AuthContext';
import { UnknownObject } from '@/types/common';

import { POP_UP_TITLE, UNSAVED_DATA_MESSAGE } from './constants';
import { FormikWithBackupInnerProps, FormikWithBackupProps } from './types';
import {
	isEqual,
	getFormKey,
	getDataFromStorage,
	putDataInStorage,
	removeDataFromStorage,
} from './utils';

function FormikWithBackupInner<Values extends FormikValues>({
	formikProps,
	children,
	name,
	id,
	onClose,
	valuesKeysExcludedFromBackup,
}: FormikWithBackupInnerProps<Values>) {
	const { initialValues, values, setValues, dirty } = formikProps;
	const [open, setOpen] = useState(false);
	const valuesRef = useRef(values);
	const { setLogoutCallback, id: userId } = useAuth();
	const formKey = useMemo(
		() => getFormKey({ userId, name, id }),
		[userId, name, id]
	);

	useEffect(() => {
		const savedValues = getDataFromStorage(formKey);

		if (savedValues) {
			setOpen(true);
		}
	}, [formKey]);

	useEffect(() => {
		const saveFormValues = () => {
			const snapshot = { ...valuesRef.current } as UnknownObject;

			if ('files' in snapshot) {
				delete snapshot.files;
			}

			valuesKeysExcludedFromBackup?.forEach((key) => {
				delete snapshot[key];
			});

			if (!isEqual(snapshot, initialValues)) {
				putDataInStorage(snapshot, formKey);
			}
		};

		window.addEventListener('beforeunload', saveFormValues);
		setLogoutCallback(saveFormValues);

		return () => {
			window.removeEventListener('beforeunload', saveFormValues);
			setLogoutCallback(null);
		};
	}, [initialValues, formKey, valuesKeysExcludedFromBackup]);

	useEffect(() => {
		valuesRef.current = values;
	}, [values]);

	const closePopUp = () => {
		setOpen(false);
	};

	const restoreDataAndClosePopUp = () => {
		const savedValues = getDataFromStorage(formKey);

		if (!savedValues) {
			closePopUp();

			return;
		}

		const merged = { ...savedValues } as UnknownObject;

		valuesKeysExcludedFromBackup?.forEach((key) => {
			delete merged[key];
		});

		setValues((old) => ({ ...old, ...merged }));
		removeDataFromStorage(formKey);
		closePopUp();
	};

	const deleteDataAndClosePopup = () => {
		removeDataFromStorage(formKey);
		closePopUp();
	};

	const elements = (
		<FormWithSaveExit isFormDirty={dirty} onClose={onClose}>
			{(handleOutsideClose) =>
				typeof children === 'function'
					? children({ ...formikProps, handleOutsideClose })
					: children
			}
		</FormWithSaveExit>
	);

	if (open) {
		return (
			<>
				{elements}
				<PopUp
					title={POP_UP_TITLE}
					zIndex={zIndexes.confirmationPopup}
					controls={{
						positiveControl: {
							onClick: restoreDataAndClosePopUp,
						},
						negativeControl: {
							onClick: deleteDataAndClosePopup,
						},
					}}
					onClose={deleteDataAndClosePopup}
				>
					{UNSAVED_DATA_MESSAGE}
				</PopUp>
			</>
		);
	}

	return elements;
}

export function FormikWithBackup<Values extends FormikValues, ExtraProps>({
	children,
	name,
	id,
	onClose,
	valuesKeysExcludedFromBackup,
	...rest
}: FormikWithBackupProps<Values, ExtraProps>) {
	return (
		<Formik {...rest}>
			{(formikProps: FormikProps<Values>) => (
				<FormikWithBackupInner
					{...{
						formikProps,
						name,
						id,
						onClose,
						valuesKeysExcludedFromBackup,
					}}
				>
					{children}
				</FormikWithBackupInner>
			)}
		</Formik>
	);
}
