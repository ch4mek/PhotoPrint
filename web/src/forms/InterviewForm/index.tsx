import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { TimeInput } from '@mms/mms-ui-library/components/TimeInput';
import { useMemo } from 'react';

import { FormikWithBackup } from '../components/FormikWithBackup';

import { isSaleManager } from '@/constants/departments';
import { isRegularEmployee } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { TextField } from '@/fields/Text';
import { TextArea } from '@/fields/TextArea';
import { extractKeyById } from '@/helpers/extractKey';
import { getFullName } from '@/helpers/utils';
import { useResize } from '@/hooks/useResize';
import dayjs from '@/lib/dayjs';
import { useInterviewUsersLookup } from '@/queries/interview/useInterviewUsersLookup';
import { useSaleManagersLookup } from '@/queries/interview/useSaleManagersLookup';

import { FORM_TITLE, interviewStatuses, interviewTypes } from './constants';
import { useSendInterviewForm } from './hooks/useSendInterviewForm';
import { schema } from './schema';
import { FieldsWrapper, DateTimeWrapper, getPopUpStylesConfig } from './styles';
import { InterviewFormProps, InterviewFormValues } from './types';
import {
	getFormTitle,
	getInitialValues,
	getUserOptions,
	convertFormValues,
} from './utils';

const TEXTAREA_HEIGHT = 89;

export function InterviewForm({
	interview,
	onClose,
	parentInterview,
	preselectedUser,
}: InterviewFormProps) {
	const theme = useTheme();
	const [windowWidth, windowHeight] = useResize();
	const {
		id,
		departmentId,
		firstName,
		lastName,
		firstNameRu,
		lastNameRu,
		role,
	} = useAuth();

	const { data: users, isLoading: isUsersLookupLoading } =
		useInterviewUsersLookup(true, { enabled: !preselectedUser });

	const { data: saleManagers, isLoading: isSaleManagersLookupLoading } =
		useSaleManagersLookup({
			onlyActive: true,
		});

	const {
		isLoading: isSubmitting,
		handleAddInterview,
		handleEditInterview,
		handleAddInterviewStage,
	} = useSendInterviewForm(
		onClose,
		interview?.groupId ?? parentInterview?.groupId
	);

	const userOptions = useMemo(() => getUserOptions(users), [users]);
	const saleManagerOptions = useMemo(
		() => getUserOptions(saleManagers),
		[saleManagers]
	);

	const isCurrentUserSaleManagerEmployee =
		isSaleManager(departmentId) && isRegularEmployee(role);

	const currentUserFullName = getFullName(
		firstName,
		lastName,
		firstNameRu,
		lastNameRu
	);

	const initialValues = useMemo(
		() =>
			getInitialValues({
				isSaleManager: isCurrentUserSaleManagerEmployee,
				saleManagerOption: { id, name: currentUserFullName },
				interview,
				parentInterview,
				preselectedUser,
			}),
		[
			isCurrentUserSaleManagerEmployee,
			id,
			currentUserFullName,
			interview,
			parentInterview,
			preselectedUser,
		]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowWidth, windowHeight),
		[theme, windowWidth, windowHeight]
	);

	const handleSubmit = async (values: InterviewFormValues) => {
		const convertedValues = convertFormValues(values);

		if (interview) {
			await handleEditInterview({
				interviewId: interview.id,
				...convertedValues,
			});
		} else if (parentInterview) {
			await handleAddInterviewStage(convertedValues);
		} else {
			await handleAddInterview(convertedValues);
		}
	};

	const showLoader =
		isUsersLookupLoading || isSaleManagersLookupLoading || isSubmitting;

	return (
		<FormikWithBackup
			enableReinitialize
			validateOnBlur={false}
			validationSchema={schema}
			onSubmit={handleSubmit}
			name={FORM_TITLE}
			id={interview?.id}
			{...{ initialValues, onClose }}
		>
			{({
				values,
				errors,
				touched,
				setFieldValue,
				setFieldTouched,
				submitForm,
				handleOutsideClose,
			}) => {
				const handleChange = (
					fieldName: string | undefined,
					value: InterviewFormValues[keyof InterviewFormValues]
				) => {
					if (fieldName) {
						setFieldValue(fieldName, value);
						setFieldTouched(fieldName);
					}
				};

				const handleChangeCurried =
					(fieldName: string) =>
					(value: InterviewFormValues[keyof InterviewFormValues]) => {
						handleChange(fieldName, value);
					};

				return (
					<PopUp
						title={getFormTitle(!!interview)}
						type="form"
						onClose={onClose}
						onShadowClick={handleOutsideClose}
						showLoader={showLoader}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: onClose },
						}}
						stylesConfig={popUpStylesConfig}
					>
						<FieldsWrapper>
							<Autoselect
								name="user"
								isRequired
								valueField="name"
								value={values.user}
								options={userOptions}
								label="User"
								error={!!errors.user && touched.user}
								errorText={errors.user}
								onChange={handleChangeCurried('user')}
								getKey={extractKeyById}
								disabled={!!preselectedUser}
							/>
							<TextField
								name="specialization"
								isRequired
								value={values.specialization}
								label="Specialization"
								error={!!errors.specialization && touched.specialization}
								errorText={errors.specialization}
								onChange={handleChange}
							/>
							<TextField
								name="client"
								value={values.client}
								label="Client"
								error={!!errors.client && touched.client}
								errorText={errors.client}
								onChange={handleChange}
							/>
							<TextField
								name="jiraTask"
								value={values.jiraTask}
								label="Jira Task"
								error={!!errors.jiraTask && touched.jiraTask}
								errorText={errors.jiraTask}
								onChange={handleChange}
							/>
							<Autoselect
								name="type"
								valueField="name"
								value={values.type}
								options={interviewTypes}
								label="Type"
								error={!!errors.type && touched.type}
								errorText={errors.type}
								onChange={handleChangeCurried('type')}
								getKey={extractKeyById}
							/>
							<Autoselect
								name="status"
								valueField="name"
								value={values.status}
								options={interviewStatuses}
								label="Status"
								error={!!errors.status && touched.status}
								errorText={errors.status}
								onChange={handleChangeCurried('status')}
								getKey={extractKeyById}
							/>
							<DateTimeWrapper>
								<DatePicker
									value={dayjs(values.date)}
									innerLabel="Date"
									errorText={errors.date}
									onChange={handleChangeCurried('date')}
									size="fullWidth"
								/>
								<TimeInput
									name="time"
									size="fullWidth"
									value={values.time}
									label="Time"
									error={!!errors.time && touched.time}
									errorText={errors.time}
									onChange={handleChange}
								/>
							</DateTimeWrapper>
							<Autoselect
								name="saleManager"
								valueField="name"
								value={values.saleManager}
								options={saleManagerOptions}
								label="Sale manager"
								error={!!errors.saleManager && touched.saleManager}
								errorText={errors.saleManager}
								onChange={handleChangeCurried('saleManager')}
								getKey={extractKeyById}
								disabled={isCurrentUserSaleManagerEmployee}
							/>
							<TextField
								name="reasonForRefusal"
								value={values.reasonForRefusal}
								label="Reason for refusal"
								error={!!errors.reasonForRefusal && touched.reasonForRefusal}
								errorText={errors.reasonForRefusal}
								onChange={handleChange}
							/>
							<TextArea
								name="note"
								size="fullWidth"
								label="Note"
								style={{ height: TEXTAREA_HEIGHT }}
								error={!!errors.note && touched.note}
								errorText={errors.note}
								onChange={handleChange}
							/>
						</FieldsWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
