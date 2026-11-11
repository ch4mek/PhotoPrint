import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { months } from '@/constants/date';
import { fileExtensions } from '@/constants/fileExtensions';
import { ReportsQueriesKeys } from '@/constants/queriesKeys';
import {
	GET_REPORTS_EXCEL,
	GET_REPORTS_WORD,
	GET_REPORTS_PDF,
	GET_REPORTS_EXCEL_SECOND_VERSION,
} from '@/constants/queryPaths';
import { Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ToastReponseWithBody } from '@/types/common';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { ExportConfig, ExportOptions } from './types';

const config: ExportConfig = {
	url: {
		[fileExtensions.xlsx]: GET_REPORTS_EXCEL,
		[fileExtensions.excelAccount]: GET_REPORTS_EXCEL_SECOND_VERSION,
		[fileExtensions.word]: GET_REPORTS_WORD,
		[fileExtensions.pdf]: GET_REPORTS_PDF,
	},
	headers: {
		[fileExtensions.excelAccount]: {
			'Content-type': 'application/vnd.ms-excel',
			'Content-Disposition': 'attachment',
		},
		[fileExtensions.xlsx]: {
			'Content-type': 'application/vnd.ms-excel',
			'Content-Disposition': 'attachment',
		},
		[fileExtensions.pdf]: {
			'Content-type': 'application/pdf',
			'Content-Disposition': 'attachment',
		},
		[fileExtensions.word]: {
			'Content-type': 'text/docx',
			'Content-Disposition': 'attachment',
		},
	},
};

export const useExport = (
	params: ExportOptions,
	options?: PatchedQueryOptions<ToastReponseWithBody, AxiosError>
) => {
	const { role, id } = useAuth();
	const { SE } = Roles;

	const queryFn = async () => {
		try {
			const {
				year,
				monthOfYear,
				departmentId,
				clientId,
				projectId,
				file,
				userId,
				trackedTimeType,
				groupId,
				isPerUserExport,
			} = params;

			const { data } = await axios.get<ToastReponseWithBody>(
				config.url[file as string],
				{
					...config.headers[file as string],
					params: {
						year,
						monthOfYear:
							(monthOfYear &&
								monthOfYear.map(
									(month) => months.findIndex((m) => m === month) + 1
								)) ||
							null,
						departmentId,
						clientId,
						projectId,
						resourceGroupId: groupId,
						userId: role === SE ? id : userId,
						trackedTimeType,
						isPerUserExport,
					},
					paramsSerializer: {
						indexes: true,
					},
				}
			);

			return data;
		} catch (error) {
			return Promise.reject(error);
		}
	};

	return useQueryWithCallback<ToastReponseWithBody, AxiosError>({
		queryKey: [ReportsQueriesKeys.reportsDocument, params, params.file],
		...options,
		queryFn,
		enabled: Boolean(params.file),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		retry: 0,
	});
};
