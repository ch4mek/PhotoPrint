import { useTranslation } from 'react-i18next';

import { Widget } from '@/components/Widget';

import { LinkItem } from './components/LinkItem';
import { COMPANY_LINKS_WIDGET_HEIGHT, companyLinks } from './constants';
import { CompanyLinksWidgetWrapper } from './styles';

export function CompanyLinksWidget() {
	const { t } = useTranslation('home', { keyPrefix: 'widgets.companyLinks' });

	return (
		<Widget {...{ title: t('title') }} height={COMPANY_LINKS_WIDGET_HEIGHT}>
			<CompanyLinksWidgetWrapper>
				{companyLinks.map((item) => (
					<LinkItem key={item.title} {...item} />
				))}
			</CompanyLinksWidgetWrapper>
		</Widget>
	);
}
