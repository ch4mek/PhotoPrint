import { useMemo } from 'react';

import { UseJiraSettingsRowsProps } from '../types';

import { JiraSettingsGridRow } from '@/types/jira';

export const useJiraSettingsRows = ({
	items,
	dataUpdatedAt,
	page,
	rowsPerPage,
}: UseJiraSettingsRowsProps): Array<JiraSettingsGridRow> =>
	useMemo(
		() =>
			(items || []).map(
				({
					id,
					url,
					email,
					jiraProjectTag,
					projectName,
				}): JiraSettingsGridRow => ({
					id,
					url,
					email,
					projectTag: jiraProjectTag,
					projectName: projectName ?? '',
					pat: '',
				})
			),
		[dataUpdatedAt, page, rowsPerPage, items]
	);
