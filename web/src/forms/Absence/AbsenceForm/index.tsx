import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useCallback, useMemo, useRef, useState } from 'react';

import {
	ABSENCE_ENTITY_NAME,
	absencesType,
	accrualsType,
	activitiesType,
	trackingUnitsType,
	transfersType,
} from '@/constants/absence';
import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { controlsText } from '@/constants/controlsTexts';
import {
	NotificationType,
	SAME_ABSENCE_NAME_ALREADY_EXISTS,
} from '@/constants/notifications';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import {
	AbsenceTabs,
	absenceTabsLabels,
	initialFormValues,
	FORM_NAME,
} from '@/forms/Absence/AbsenceForm/constants';
import { getPopUpStylesConfig } from '@/forms/Absence/AbsenceForm/styles';
import {
	AbsenceFormProps,
	AbsenceFormValues,
} from '@/forms/Absence/AbsenceForm/types';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { createToast } from '@/helpers/createToast';
import { useResize } from '@/hooks/useResize';
import { useGetAbsenceById } from '@/queries/absences/useGetAbsenceById';

import { AbsenceFormInner } from './components/AbsenceFormInner';
import { convertDataToOptions } from './components/DetailsTab/utils/convertDataToOptions';
import { createAbsenceValidationSchema } from './schema';
import { convertDataToSubmit } from './utils/convertDataToSubmit';
import { convertInitialDataToForm } from './utils/convertInitialDataToForm';

const { Details, Approvals } = AbsenceTabs;

export function AbsenceForm({
	id,
	isLoading,
	handleFormSubmit,
	handleClose,
	checkIsNameExist,
}: AbsenceFormProps) {
	const theme = useTheme();
	const toast = useToast();
	const {
		persistentValues: {
			activityTypes,
			trackingUnitTypes,
			absenceTypes,
			accrualTypes,
			transferTypes,
		},
	} = useConfiguration();
	const [, windowHeight] = useResize();

	const [activeTab, setActiveTab] = useState(Details);
	const [isDetailsTabValid, setIsDetailsTabValid] = useState(false);
	const [isNameExists, setIsNameExists] = useState(false);

	const submitFormButtonRef = useRef<HTMLButtonElement>(null);
	const formikRef =
		useRef<FormikWithBackupRenderProps<AbsenceFormValues>>(null);

	const isEditPopUp = !!id;
	const absenceId = id;

	const { data, isLoading: isDataLoading } = useGetAbsenceById(absenceId, {
		enabled: isEditPopUp,
	});

	const validationSchema = useMemo(
		() => createAbsenceValidationSchema(checkIsNameExist, data?.id),
		[checkIsNameExist, data?.id]
	);

	const { absenceTypesOptions, activityTypesOptions, accrualTypeOptions } =
		useMemo(
			() => ({
				absenceTypesOptions: [
					...convertDataToOptions(absencesType, absenceTypes),
				],
				activityTypesOptions: convertDataToOptions(
					activitiesType,
					activityTypes
				),
				accrualTypeOptions: convertDataToOptions(accrualsType, accrualTypes),
			}),
			[absenceTypes, activityTypes, accrualTypes]
		);

	const initialData = useMemo(() => {
		if (!data) {
			return initialFormValues;
		}

		const allTrackingUnitTypesOptions = convertDataToOptions(
			trackingUnitsType,
			trackingUnitTypes
		);
		const allTransferTypeOptions = convertDataToOptions(
			transfersType,
			transferTypes
		);

		return convertInitialDataToForm(
			data,
			absenceTypesOptions,
			activityTypesOptions,
			allTrackingUnitTypesOptions,
			accrualTypeOptions,
			allTransferTypeOptions
		);
	}, [data, absenceTypesOptions, activityTypesOptions, accrualTypeOptions]);

	const absenceTabs = useMemo(
		() => [
			{
				id: Details,
				text: absenceTabsLabels[Details],
			},
			{
				id: Approvals,
				text: absenceTabsLabels[Approvals],
				disabled: !isDetailsTabValid || isNameExists,
			},
		],
		[isDetailsTabValid, isNameExists]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, activeTab, windowHeight),
		[theme, activeTab, windowHeight]
	);

	const popUpTitle = isEditPopUp
		? getEditingPopUpTitle(ABSENCE_ENTITY_NAME)
		: getAddingPopUpTitle(ABSENCE_ENTITY_NAME);

	const handleNextTab = useCallback(async () => {
		if (!formikRef.current) {
			return;
		}

		const isNameExist = checkIsNameExist(
			formikRef.current.values.details.name,
			data?.id
		);

		if (isNameExist) {
			toast.open(
				createToast(NotificationType.ERROR, SAME_ABSENCE_NAME_ALREADY_EXISTS)
			);

			return;
		}

		const errors = await formikRef.current.validateForm();
		const hasDetailsErrors =
			errors.details && Object.keys(errors.details).length > 0;

		if (!hasDetailsErrors) {
			setActiveTab(Approvals);
		} else if (errors.details) {
			formikRef.current.setTouched({
				details: Object.keys(errors.details).reduce(
					(acc, key) => ({ ...acc, [key]: true }),
					{}
				),
			});
		}
	}, [data?.id]);

	const handlePrevTab = useCallback(() => {
		setActiveTab(Details);
	}, []);

	const popUpControls = useMemo(() => {
		if (activeTab === Details) {
			return {
				positiveControl: {
					text: controlsText.nextText,
					onClick: handleNextTab,
				},
				extraControls: [
					{
						text: controlsText.cancelText,
						view: 'filled' as const,
						color: 'secondary' as const,
						onClick: handleClose,
					},
				],
			};
		}

		return {
			positiveControl: {
				text: controlsText.applyText,
				onClick: () => {
					submitFormButtonRef?.current?.click();
				},
			},
			extraControls: [
				{
					text: controlsText.backText,
					view: 'filled' as const,
					color: 'secondary' as const,
					onClick: handlePrevTab,
				},
				{
					text: controlsText.cancelText,
					view: 'filled' as const,
					color: 'secondary' as const,
					onClick: handleClose,
				},
			],
		};
	}, [activeTab, handleClose, handleNextTab, handlePrevTab]);

	const handleSubmit = useCallback(
		async (values: AbsenceFormValues) => {
			await handleFormSubmit(convertDataToSubmit(values));
			handleClose();
		},
		[handleFormSubmit, handleClose]
	);

	const handleChangeActiveTab = useCallback((newValue: number) => {
		setActiveTab(newValue);
	}, []);

	return (
		<FormikWithBackup
			innerRef={formikRef}
			initialValues={initialData}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={data?.id}
			enableReinitialize
			validateOnChange
		>
			{(formikProps) => (
				<PopUp
					type="form"
					showLoader={isLoading || isDataLoading}
					title={popUpTitle}
					onClose={handleClose}
					onShadowClick={formikProps.handleOutsideClose}
					controls={popUpControls}
					stylesConfig={popUpStylesConfig}
					tabs={{
						data: absenceTabs,
						activeTab,
						onChangeTab: handleChangeActiveTab,
					}}
				>
					<AbsenceFormInner
						{...{
							formikProps,
							setIsDetailsTabValid,
							setIsNameExists,
							checkIsNameExist,
							dataId: data?.id,
							dataHasAnyTransactions: data?.hasAnyTransactions,
							absenceTypesOptions,
							activityTypesOptions,
							accrualTypeOptions,
							activeTab,
							submitFormButtonRef,
						}}
					/>
				</PopUp>
			)}
		</FormikWithBackup>
	);
}
