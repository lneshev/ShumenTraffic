import Article from "@/components/info/Article";
import { Metadata } from "next";

export const metadata: Metadata = {
  title: 'Информация - Шумен Трафик'
}

const news = [
  {
    title: "Маршрутни промени по линия 7А",
    date: "2025-09-12",
    content: "От 12.09.2025 г. линия 7А ще започне да се движи по нов маршрут. Новият маршрут ще включва в зависимост от посоката."
  }
];

export default function InfoPage() {
  return (
    <div className="bg-background">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <h1 className="text-4xl font-bold text-foreground mb-4">
          Information
        </h1>

        {/* Latest News */}
        <section className="mb-8">
          <h2 className="text-2xl font-bold text-foreground mb-6">
            Latest News
          </h2>
          <div className="space-y-6">
            {true ? (
              <article className="p-6 bg-background-secondary rounded-lg border border-border">
                No news
              </article>
            ) : (
              news.map(n => (
                <Article key={n.title} title={n.title} date={n.date}>
                  {n.content}
                </Article>
              ))
            )}
          </div>
        </section>

        {/* General Information for buses in Shumen */}
        <section className="mb-8">
          <h2 className="text-2xl font-bold text-foreground mb-6">
            General Information for buses in Shumen
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="p-6 bg-orange-50 dark:bg-orange-900/20 rounded-lg border border-orange-200 dark:border-orange-800">
              <h3 className="text-lg font-semibold text-orange-900 dark:text-orange-100 mb-3">
                Contact institutions
              </h3>
              <p className="text-orange-800 dark:text-orange-200">
                Transportation company's website: <a href="https://shumenpat.com/" target="_blank" className="text-blue-600 dark:text-blue-400 hover:underline">https://shumenpat.com/</a>
                <br />
                Shumen municipality's website: <a href="https://shumen.bg" target="_blank" className="text-blue-600 dark:text-blue-400 hover:underline">https://shumen.bg</a>
              </p>
            </div>
            <div className="p-6 bg-green-50 dark:bg-green-900/20 rounded-lg border border-green-200 dark:border-green-800">
              <h3 className="text-lg font-semibold text-green-900 dark:text-green-100 mb-3">
                Ticket Information
              </h3>
              <p className="text-green-800 dark:text-green-200">
                Single ticket: 0.50 €
                <br />
                Buy from the bus conductor.
              </p>
            </div>
          </div>
        </section>

        {/* About Shumen Traffic */}
        <section>
          <h2 className="text-2xl font-bold text-foreground mb-6">
            About Shumen Traffic
          </h2>
          <div className="p-6 bg-background-secondary rounded-lg border border-border">
            {true ? (
              <p>
                Shumen Traffic is a website for tracking city buses in the city of Shumen, and more precisely their schedules, routes and stops.
                <br />
                <br />
                The website was created and maintained voluntarily for the benefit of the residents and guests of the city of Shumen. It is not funded in any way by official institutions and authorities!
                <br />
                <br />
                The data is collected from official sources, media and volunteers. It is updated frequently, but there is no guarantee of its accuracy. The website is not responsible for any troubles that may occur due to inaccurate or outdated data.
              </p>
            ) : (
              <p>
                Shumen Traffic е уебсайт за проследяване на градските автобуси в град Шумен и по-точно техните разписания, маршрути и спирки.
                <br />
                Уебсайтът е създаден и поддържан доброволно в полза на жителите и гостите на град Шумен. Не е финансиран по никакъв начин от официалните институции и органи!
                <br />
                Данните се събират от официални източници, медии и доброволци. Обновяват се често, но няма гаранция за точността им. Уебсайтът не носи отговорност за неприятности, които може да се случат поради неточни или неактуални данни.
              </p>
            )}
          </div>
        </section>
      </div>
    </div>
  );
}