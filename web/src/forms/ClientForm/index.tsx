import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo, useState } from 'react';

import { ActivationMessage } from '@/components/ActivationMessage';
import { COLOR_PICKER_FIELD_NUMBER_OF_COLORS } from '@/components/ColorPickerControl/constants';
import {
	ACTIVATE,
	DEACTIVATE,
	getActivatingPopUpTitle,
	getAddingPopUpTitle,
	getDeactivatingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import {
	CLIENT_CANNOT_BE_DEACTIVATED,
	CLIENT_CANNOT_BE_DEACTIVATED_TOOLTIP,
	SOMETHING_WENT_WRONG,
} from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { popUpBreakpoints } from '@/constants/popUp';
import { ClientQuerieKeys } from '@/constants/queriesKeys';
import { isSysAdmin } from '@/constants/roles';
import { zIndexes } from '@/constants/zIndexes';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { ColorPickerField } from '@/fields/ColorPickerField';
import { TextField } from '@/fields/Text';
import { TextArea } from '@/fields/TextArea';
import type {
	ClientFormProps,
	ClientFormValues,
} from '@/forms/ClientForm/types';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { createToast } from '@/helpers/createToast';
import { removeExtraSpaces } from '@/helpers/removeExtraSpaces';
import { useResize } from '@/hooks/useResize';
import { useSaleManagersLookup } from '@/queries/interview/useSaleManagersLookup';
import { useClient } from '@/queries/useClient';
import { useClientProjects } from '@/queries/useClientProjects';
import { useUpdateClientStatus } from '@/queries/useUpdateClientStatus';
import { CalendaryCountry } from '@/types/configuration';

import {
	ENTITY_NAME,
	TEXTAREA_MIN_ROWS,
	FORM_NAME,
	CLIENT_FORM_FIELDS,
} from './constants';
import { validationSchema } from './schema';
import { FormContentWrapper } from './styles';
import {
	getPopUpStylesConfig,
	mapClientToFormValues,
	mapSaleManagersToOptions,
} from './utils';

export function ClientForm({
	initialValues,
	handleClose,
	onSubmit,
	id,
	refetch,
}: ClientFormProps) {
	const [operationName, setOperationName] = useState<
		'activate' | 'deactivate' | null
	>(null);

	const toast = useToast();
	const theme = useTheme();
	const [, windowHeight] = useResize();
	const { role } = useAuth();
	const isUserSysAdmin = isSysAdmin(role);
	const {
		configurableValues: { countries },
	} = useConfiguration();

	const { data: client, isFetching } = useClient(id);

	const { isActive: isClientActive = false, name: clientName = '' } =
		client || {};

	const isEditMode = Boolean(id);
	const isFormDisabled = isEditMode ? !client?.isActive : false;
	const isMobileLWidth = useHasScreenWidth(popUpBreakpoints.mobileL);

	const { data: clientProjects, isFetching: areProjectsFetching } =
		useClientProjects(id!, undefined, {
			enabled: isEditMode && client?.isActive,
			queryKey: [ClientQuerieKeys.clientProjectsForForm, id],
		});

	const onChangeActiveStatus = useUpdateClientStatus({
		onSuccess() {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					`Client is successfully ${
						client?.isActive ? 'deactivated' : 'activated'
					}`
				)
			);
			refetch();
		},
		onError() {
			toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));
		},
	});

	const { data: saleManagers, isLoading: isSaleManagersLookupLoading } =
		useSaleManagersLookup({
			onlyActive: true,
		});

	const saleManagerOptions = useMemo(
		() => mapSaleManagersToOptions(saleManagers),
		[saleManagers]
	);

	const handleCloseDialog = useCallback(() => setOperationName(null), []);

	const handleDeactivateButtonClick = useCallback(() => {
		if (!isEditMode || !client) {
			return;
		}

		setOperationName(isClientActive ? 'deactivate' : 'activate');
	}, [isEditMode, client]);

	const handleChangeClientStatus = useCallback(() => {
		if (!operationName || !id || !client) {
			return;
		}

		handleCloseDialog();

		if (isClientActive && clientProjects?.length) {
			toast.open(
				createToast(NotificationType.ERROR, CLIENT_CANNOT_BE_DEACTIVATED)
			);

			return;
		}

		handleClose();

		if (operationName === 'activate') {
			onChangeActiveStatus.mutate({
				id,
				newStatus: true,
			});
		}

		if (operationName === 'deactivate') {
			onChangeActiveStatus.mutate({
				id,
				newStatus: false,
			});
		}
	}, [id, operationName, client, clientProjects]);

	const handleSubmit = useCallback(
		async (
			values: ClientFormValues,
			{ setSubmitting }: FormikHelpers<ClientFormValues>
		) => {
			const { saleManager } = values;
			const { registeredLocation, ...rest } = removeExtraSpaces(
				values,
				validationSchema
			) as unknown as ClientFormValues;

			const registeredLocationId = countries.find(
				(item) => item.name === registeredLocation
			)?.id;

			if (registeredLocationId === undefined) {
				setSubmitting(false);

				return;
			}

			try {
				await onSubmit({
					...rest,
					salesManagerId: saleManager?.id ?? null,
					registeredLocation: registeredLocationId,
				});
			} finally {
				setSubmitting(false);
			}
		},
		[onSubmit, countries]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const resolvedInitialValues = useMemo(() => {
		if (client) {
			return mapClientToFormValues(client);
		}

		return initialValues;
	}, [client, initialValues]);

	return (
		<>
			<FormikWithBackup
				enableReinitialize
				initialValues={resolvedInitialValues}
				validationSchema={validationSchema}
				onSubmit={handleSubmit}
				onClose={handleClose}
				validateOnBlur={false}
				name={FORM_NAME}
				id={id}
			>
				{({
					isSubmitting,
					values,
					setFieldValue,
					validateField,
					submitForm,
					setFieldTouched,
					errors,
					touched,
					handleOutsideClose,
				}) => {
					const handleOnChangeRegisteredValue =
						(field: string) => (value: unknown) => {
							const registeredLocationValue = value as CalendaryCountry;

							if (value) {
								setFieldValue(field, registeredLocationValue.name);
							} else {
								setFieldValue(field, '');
							}
							setFieldTouched(field, true);
						};

					const handleAutoselectChange =
						(field: string) => (value: unknown) => {
							setFieldValue(field, value);
							setFieldTouched(field, true);
							validateField(field);
						};

					const showLoader =
						Boolean(isFetching && id) ||
						isSubmitting ||
						areProjectsFetching ||
						isSaleManagersLookupLoading;

					return (
						<PopUp
							onClose={handleClose}
							onShadowClick={handleOutsideClose}
							title={
								id
									? getEditingPopUpTitle(ENTITY_NAME)
									: getAddingPopUpTitle(ENTITY_NAME)
							}
							type="form"
							controls={{
								extraControls: client
									? [
											{
												view: 'light',
												text: isClientActive ? DEACTIVATE : ACTIVATE,
												color: isClientActive ? 'negative' : 'primary',
												onClick: handleDeactivateButtonClick,
											},
									  ]
									: undefined,
								positiveControl: {
									onClick: submitForm,
									disabled: isFormDisabled,
									tooltipText: isFormDisabled
										? CLIENT_CANNOT_BE_DEACTIVATED_TOOLTIP
										: undefined,
								},
								negativeControl: { onClick: handleClose },
							}}
							showLoader={showLoader}
							stylesConfig={popUpStylesConfig}
						>
							<FormContentWrapper>
								<TextField
									{...CLIENT_FORM_FIELDS.name}
									size="fullWidth"
									onChange={setFieldValue}
									disabled={isFormDisabled}
									isRequired
								/>
								<TextField
									{...CLIENT_FORM_FIELDS.legalEntityName}
									size="fullWidth"
									onChange={setFieldValue}
									disabled={isFormDisabled}
								/>
								<Autoselect
									{...CLIENT_FORM_FIELDS.registeredLocation}
									disabled={isUserSysAdmin || isFormDisabled}
									value={values.registeredLocation}
									onChange={handleOnChangeRegisteredValue(
										CLIENT_FORM_FIELDS.registeredLocation.name
									)}
									error={
										Boolean(errors.registeredLocation) &&
										touched.registeredLocation
									}
									errorText={errors.registeredLocation}
									options={countries}
									isRequired
								/>
								<Autoselect
									{...CLIENT_FORM_FIELDS.saleManager}
									disabled={isFormDisabled}
									value={values.saleManager}
									onChange={handleAutoselectChange(
										CLIENT_FORM_FIELDS.saleManager.name
									)}
									error={Boolean(errors.saleManager) && touched.saleManager}
									errorText={errors.saleManager}
									options={saleManagerOptions}
								/>
								<TextArea
									{...CLIENT_FORM_FIELDS.comments}
									size="fullWidth"
									disabled={isFormDisabled}
									minRows={TEXTAREA_MIN_ROWS}
								/>
								<ColorPickerField
									{...CLIENT_FORM_FIELDS.color}
									value={values.color}
									disabled={isFormDisabled}
									flexible
									spaceBetween
									numberOfColors={
										isMobileLWidth
											? COLOR_PICKER_FIELD_NUMBER_OF_COLORS.mobileL
											: COLOR_PICKER_FIELD_NUMBER_OF_COLORS.mobile
									}
								/>
							</FormContentWrapper>
						</PopUp>
					);
				}}
			</FormikWithBackup>

			{client && operationName && (
				<PopUp
					title={
						isClientActive
							? getDeactivatingPopUpTitle(ENTITY_NAME)
							: getActivatingPopUpTitle(ENTITY_NAME)
					}
					onClose={handleCloseDialog}
					type="approve"
					showCloseButton={false}
					controls={{
						positiveControl: { onClick: handleChangeClientStatus },
						negativeControl: { onClick: handleCloseDialog },
					}}
					zIndex={zIndexes.formPopup}
				>
					<ActivationMessage
						isActive={isClientActive}
						name={clientName}
						entityName={ENTITY_NAME.toLowerCase()}
					/>
				</PopUp>
			)}
		</>
	);
}
