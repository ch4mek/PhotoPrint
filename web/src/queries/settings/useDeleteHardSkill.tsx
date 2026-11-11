import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import {
	NotificationType,
	ITEM_DELETED_POSTFIX,
} from '@/constants/notifications';
import { GET_HARD_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { SkillName } from '@/pages/Settings/components/SkillsTab/constants';
import { HardSkill } from '@/types/settings';

export const useDeleteHardSkill = (toast: ToastContextValues) =>
	useMutation({
		mutationFn: async ({ id }: HardSkill) => {
			try {
				const { data } = await axios.delete(
					`${GET_HARD_SKILLS_API_ENDPOINT}/${id}`
				);
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						`${capitalizeString(SkillName.HardSkill)} ${ITEM_DELETED_POSTFIX}`
					)
				);

				return data;
			} catch (error) {
				return Promise.reject(error);
			}
		},
		onError: (error: AxiosError<ServerResponseError>) => {
			createOnErrorHandler(toast)(error);
		},
	});
