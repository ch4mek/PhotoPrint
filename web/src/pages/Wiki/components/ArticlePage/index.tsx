import { useEffect, useMemo, useState } from 'react';
import { useParams } from 'react-router-dom';

import { useWikiContext } from '../../context/wikiContext';
import { PlateEditor } from '../PlateEditor';

import { Loader } from '@/components/Loader';
import { initialEditorData } from '@/constants/plateEditor';
import { checkIsEditorEmpty } from '@/pages/Wiki/utils/checkIsEditorEmpty';
import { useGetArticleById } from '@/queries/wiki/useGetArticlesById';
import { CustomRootBlock } from '@/types/plateEditor';

import { PlateSection, PlateSectionWrapper } from './styles';

export function ArticlePage() {
	const { id } = useParams();
	const { isEditorReadOnly } = useWikiContext();
	const [parsedData, setParsedData] =
		useState<CustomRootBlock[]>(initialEditorData);

	const { data, isLoading, isFetching, refetch } = useGetArticleById(id, {
		enabled: !Number.isNaN(id),
	});

	useEffect(() => {
		if (!data) {
			return;
		}

		try {
			const parsed = data.text?.length
				? JSON.parse(data.text)
				: initialEditorData;
			setParsedData(parsed);
		} catch (error) {
			setParsedData(initialEditorData);
		}
	}, [data]);

	const isEditorEmpty = useMemo(
		() => checkIsEditorEmpty(parsedData),
		[parsedData]
	);

	return (
		<Loader isLoading={isLoading || isFetching}>
			{data && (
				<PlateSectionWrapper>
					<PlateSection isEditor={!isEditorReadOnly}>
						<PlateEditor
							currentArticleData={data!}
							currentWikiPage={parsedData}
							isEmptyArticle={isEditorEmpty}
							refetch={refetch}
						/>
					</PlateSection>
				</PlateSectionWrapper>
			)}
		</Loader>
	);
}
