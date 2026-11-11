import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { Departments } from '@/constants/departments';
import { HttpStatuses } from '@/constants/httpStatuses';
import {
	NotificationType,
	CERTIFICATE_GENERATED_SUCCESSFULLY,
	CERTIFICATE_GENERATED_ERROR,
} from '@/constants/notifications';
import { GET_CERTIFICATES_API_ENDPOINT } from '@/constants/queryPaths';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import dayjs from '@/lib/dayjs';

import { CreateCertificateOptions } from './types';

export const useCreateCertificate = ({
	onSuccess,
}: {
	onSuccess: VoidFunction;
}) => {
	const toast = useToast();

	const mutationFn = async ({
		id,
		firstName,
		lastName,
		date: receiptDate,
		department: technology,
		stack,
	}: CreateCertificateOptions) => {
		try {
			await axios.post(GET_CERTIFICATES_API_ENDPOINT, {
				recipient: `${firstName} ${lastName}`,
				receiptDate: dayjs(receiptDate).format('YYYY-MM-DD'),
				recipientId: String(id),
				technology: technology.id,
				stack: stack ? stack.id : null,
			});

			toast.open(
				createToast(
					NotificationType.SUCCESS,
					CERTIFICATE_GENERATED_SUCCESSFULLY
				)
			);

			onSuccess();
		} catch (error) {
			const statusCode = (error as AxiosError)?.response?.status;

			if (statusCode === HttpStatuses.ConflictRequest) {
				toast.open(
					createToast(
						NotificationType.ERROR,
						<>
							Employee already has{' '}
							<Typography variant="m-600">{technology.fullName}</Typography>{' '}
							certificate
							{technology.id === Departments.Js && (
								<>
									{' '}
									of{' '}
									<Typography variant="m-600">
										{stack?.description}
									</Typography>{' '}
									stack
								</>
							)}
						</>
					)
				);
			} else {
				toast.open(
					createToast(NotificationType.ERROR, CERTIFICATE_GENERATED_ERROR)
				);
			}
		}
	};

	return useMutation({ mutationFn });
};
